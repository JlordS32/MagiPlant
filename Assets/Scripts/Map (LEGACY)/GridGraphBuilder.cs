// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEditor.Experimental.GraphView;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class GridGraphBuilder : MonoBehaviour
// {
//     [Header("References")]
//     public Tilemap tilemap;

//     [Header("Tile Definitions")]
//     public TileBase grassTile;
//     public TileBase wallTile;
//     public TileBase startTile;
//     public TileBase endTile;

//     private PathTile _startNode;
//     private PathTile _endNode;


//     public PathTile[,] grid;
//     private Dictionary<TileBase, bool> walkableTilesDict;

//     void Awake()
//     {
//         SetupWalkableDictionary();
//         BuildGrid();
//     }

//     void SetupWalkableDictionary()
//     {
//         walkableTilesDict = new Dictionary<TileBase, bool>
//         {
//             { grassTile, true },
//             { wallTile, false },
//             { startTile, true },
//             { endTile, true }
//         };
//     }

//     void BuildGrid()
//     {
//         BoundsInt bounds = tilemap.cellBounds;
//         grid = new PathTile[bounds.size.x, bounds.size.y];

//         for (int x = 0; x < bounds.size.x; x++)
//         {
//             for (int y = 0; y < bounds.size.y; y++)
//             {
//                 Vector3Int tilePos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
//                 TileBase tile = tilemap.GetTile(tilePos);
//                 bool walkable = true;

//                 if (tile != null && walkableTilesDict.ContainsKey(tile))
//                     walkable = walkableTilesDict[tile];

//                 grid[x, y] = new PathTile(tilePos.x, tilePos.y, walkable);

//                 if (tile == startTile)
//                     _startNode = grid[x, y];
//                 else if (tile == endTile)
//                     _endNode = grid[x, y];

//             }
//         }

//         Debug.Log($"Grid generated with size: {bounds.size.x} x {bounds.size.y}");
//     }


//     public PathTile GetNodeFromWorld(Vector3 worldPosition)
//     {
//         Vector3Int cell = tilemap.WorldToCell(worldPosition);
//         BoundsInt bounds = tilemap.cellBounds;

//         int x = cell.x - bounds.xMin;
//         int y = cell.y - bounds.yMin;

//         if (x >= 0 && x < bounds.size.x && y >= 0 && y < bounds.size.y)
//         {
//             return grid[x, y];
//         }
//         return null;
//     }


//     public PathTile GetStartNode() => _startNode;
//     public PathTile GetEndNode() => _endNode;
//     public PathTile[,] GetGraph()
//     {
//         return grid;
//     }
// }
