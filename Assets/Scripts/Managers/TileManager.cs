using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public struct Node
{
    public TileBase Tile;
    public int Weight; // 1 = walkable, 0 = not walkable
}

public class TileManager : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] Tilemap _map;
    [SerializeField] List<Node> _nodes;

    [Header("Debug Mode")]
    [SerializeField] bool _enableDebug;

    // VARIABLES
    public int[,] Grid { get; private set; }
    public BoundsInt Bounds { get; private set; }

    public Tilemap Map => _map;

    void Update()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        // Base setup
        _map.CompressBounds(); // Important!
        Bounds = _map.cellBounds;
        Grid = new int[Bounds.size.x, Bounds.size.y];

        // Get all tile position
        foreach (Vector3Int pos in Bounds.allPositionsWithin)
        {
            TileBase tile = _map.GetTile(pos);
            int weight = -1; // Default value

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
            int x = pos.x - Bounds.xMin;
            int y = pos.y - Bounds.yMin;

            // Assign weight to grid
            Grid[x, y] = weight;
        }
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

                Gizmos.color = Grid[x, y] == 1 ? Color.green : Color.red;
                Gizmos.DrawCube(worldPos, Vector3.one * 0.8f);
            }
        }
    }

}
