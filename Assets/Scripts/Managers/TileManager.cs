using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

// WARNING: Understand the fucking code before touching, it's sensitive. Read before making any modifications!!!
[Serializable]
public struct Node
{
    public TileBase Tile;
    public TileWeight Weight;
}

public class TileManager : MonoBehaviour
{
    // STATIC
    public static event Action<TileWeight[,]> OnGridUpdated;

    // PROPERTIES
    [SerializeField] Tilemap _map;

    [Tooltip("List of nodes used for pathfinding or navigation. Think of it as a rule tile, where a tile is a node.")]
    [SerializeField] List<Node> _nodes;

    [Header("Debug Mode")]
    [SerializeField] bool _enableDebug;

    // VARIABLES
    public HashSet<Vector2Int> _occupiedTiles = new();
    public Dictionary<int, HashSet<Vector2Int>> _occupiedTileIds = new();

    // GETTERS && SETTERS
    public TileWeight[,] Grid { get; private set; }
    public BoundsInt Bounds { get; private set; }
    public Tilemap Map => _map;
    public HashSet<Vector2Int> OccupiedTiles => _occupiedTiles;
    public Dictionary<int, HashSet<Vector2Int>> OccupiedTilesIds => _occupiedTileIds;

    void Awake()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        // Base setup
        _map.CompressBounds(); // Important!
        Bounds = _map.cellBounds;
        Grid = new TileWeight[Bounds.size.x, Bounds.size.y];

        // Get all tile position
        foreach (Vector3Int pos in Bounds.allPositionsWithin)
        {
            TileBase tile = _map.GetTile(pos);
            TileWeight weight = TileWeight.Unknown; // Default value

            // Check if corresponding tile in our rule node tile.
            foreach (var node in _nodes)
            {
                // Assign weight if yes
                if (node.Tile == tile)
                {
                    weight = node.Weight;
                    break;
                }
            }

            // Normalise position to grid index.
            Vector2Int gridPos = TileToGrid(pos);

            // Assign weight to grid
            Grid[gridPos.x, gridPos.y] = weight;
        }

        OnGridUpdated?.Invoke(Grid);
    }

    public int[,] GetGridAsInt()
    {
        int width = Grid.GetLength(0);
        int height = Grid.GetLength(1);

        int[,] intGrid = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                intGrid[x, y] = (int)Grid[x, y];
            }
        }

        return intGrid;
    }

    // Gets tile cell (tilemap coordinate) from world position
    public Vector3Int WorldToTile(Vector3 worldPosition)
    {
        return _map.WorldToCell(worldPosition);
    }

    // Gets grid index ([x, y] in Grid[,]) from tile cell
    public Vector2Int TileToGrid(Vector3Int tilePos)
    {
        int x = tilePos.x - Bounds.xMin;
        int y = tilePos.y - Bounds.yMin;
        return new Vector2Int(x, y);
    }

    // Combines both function above.
    public Vector2Int WorldToGridIndex(Vector3 worldPosition)
    {
        Vector3Int tilePos = _map.WorldToCell(worldPosition);
        return TileToGrid(tilePos);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < Grid.GetLength(0) &&
                        y >= 0 && y < Grid.GetLength(1);
    }

    // For testing only
    void OnDrawGizmos()
    {
        if (Grid == null || !_enableDebug) return;

        for (int x = 0; x < Grid.GetLength(0); x++)
        {
            for (int y = 0; y < Grid.GetLength(1); y++)
            {
                // Convert back to cell position
                Vector3Int cellPos = new(x + Bounds.xMin, y + Bounds.yMin, 0);
                Vector3 worldPos = _map.CellToWorld(cellPos) + _map.cellSize / 2f;

                TileWeight weight = Grid[x, y];
                Color color = weight switch
                {
                    TileWeight.Walkable => new Color(0f, 1f, 0f, 0.5f),
                    TileWeight.Placeable => new Color(0f, 0f, 1f, 0.5f),
                    TileWeight.Blocked => new Color(1f, 0f, 0f, 0.5f),
                    _ => new Color(0f, 0f, 0f, 0.5f)
                };

                Gizmos.color = color;
                Gizmos.DrawCube(worldPos, Vector3.one * 0.8f);
            }
        }
    }

    public int SetOccupied(Vector2Int pos, TileWeight weight, int id = -1)
    {
        if (!IsInBounds(pos.x, pos.y)) return -1;

        if (id == -1)
            id = Guid.NewGuid().GetHashCode();

        Grid[pos.x, pos.y] = weight;

        if (weight != TileWeight.Walkable)
        {
            _occupiedTiles.Add(pos);
            if (id >= 0)
            {
                if (!_occupiedTileIds.ContainsKey(id))
                    _occupiedTileIds[id] = new HashSet<Vector2Int>();
                _occupiedTileIds[id].Add(pos);
            }
        }
        else
        {
            _occupiedTiles.Remove(pos);
        }

        OnGridUpdated?.Invoke(Grid);

        return id;
    }

    public int SetOccupiedArea(Vector3 areaPos, int width, int height, TileWeight weight)
    {
        int id = Guid.NewGuid().GetHashCode();

        // Get offset and position area to the bottom left.
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        Vector2Int originTile = WorldToGridIndex(areaPos) - new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new(originTile.x + x, originTile.y + y);
                SetOccupied(pos, weight, id);
            }

        return id;
    }

    public bool IsAreaValid(int width, int height, Vector3 worldPos, TileWeight required = TileWeight.Walkable)
    {
        Vector2Int origin = WorldToGridIndex(worldPos);
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        origin -= new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;

                if (!IsInBounds(gx, gy) || Grid[gx, gy] != required)
                    return false;
            }
        }

        return true;
    }

    public bool IsAreaWithinBounds(int width, int height, Vector3 worldPos)
    {
        Vector2Int origin = WorldToGridIndex(worldPos);
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        origin -= new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;

                if (!IsInBounds(gx, gy))
                {
                    Debug.LogWarning($"Out of bounds tile: {gx}, {gy}");
                    return false;
                }
            }
        }

        return true;
    }

}
