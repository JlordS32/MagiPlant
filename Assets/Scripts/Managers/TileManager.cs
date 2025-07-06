using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public struct Node
{
    public TileBase Tile;
    public TileWeight Weight;
}

public class TileManager : MonoBehaviour
{
    // STATIC
    public static event System.Action<Vector2Int, TileWeight> OnGridUpdated;

    // PROPERTIES
    [SerializeField] Tilemap _map;

    [Tooltip("List of nodes used for pathfinding or navigation. Think of it as a rule tile, where a tile is a node.")]
    [SerializeField] List<Node> _nodes;

    [Header("Debug Mode")]
    [SerializeField] bool _enableDebug;

    // VARIABLES
    public HashSet<Vector2Int> OccupiedTiles = new();

    // GETTERS && SETTERS
    public TileWeight[,] Grid { get; private set; }
    public BoundsInt Bounds { get; private set; }
    public Tilemap Map => _map;

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

                Color red = Color.red;
                red.a = 0.5f;
                Color green = Color.green;
                green.a = 0.5f;
                Gizmos.color = Grid[x, y] == TileWeight.Walkable ? green : red;
                Gizmos.DrawCube(worldPos, Vector3.one * 0.8f);
            }
        }
    }

    public void SetOccupied(Vector2Int pos, TileWeight weight)
    {
        if (!IsInBounds(pos.x, pos.y)) return;

        Grid[pos.x, pos.y] = weight;

        if (weight != TileWeight.Walkable)
            OccupiedTiles.Add(pos);
        else
            OccupiedTiles.Remove(pos);

        OnGridUpdated?.Invoke(pos, weight);
    }


    public void SetOccupiedArea(Vector3 areaPos, int width, int height, TileWeight weight)
    {
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        Vector2Int originTile = WorldToGridIndex(areaPos) - new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new(originTile.x + x, originTile.y + y);
                SetOccupied(pos, weight);
            }
        }
    }

    public void TraverseArea(int width, int height, Vector3 worldPos, System.Action<int, int> actionPerTile)
    {
        Vector2Int origin = WorldToGridIndex(worldPos);
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        origin -= new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int gx = origin.x + x;
                int gy = origin.y + y;
                if (IsInBounds(gx, gy))
                    actionPerTile?.Invoke(gx, gy);
            }
    }

    public bool IsAreaValid(int width, int height, Vector3 worldPos, TileWeight required = TileWeight.Walkable)
    {
        bool valid = true;
        TraverseArea(width, height, worldPos, (x, y) =>
        {
            if (Grid[x, y] != required)
                valid = false;
        });
        return valid;
    }

    public bool IsAreaWithinBounds(int width, int height, Vector3 worldPos)
    {
        bool within = true;
        TraverseArea(width, height, worldPos, (x, y) =>
        {
            if (!IsInBounds(x, y))
                within = false;
        });
        return within;
    }
}
