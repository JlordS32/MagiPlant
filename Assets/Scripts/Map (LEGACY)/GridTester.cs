// using UnityEngine;

// public class GridTester : MonoBehaviour
// {
//     public GridGraphBuilder gridGraph;  // Assign this in Inspector

//     void Update()
//     {
//         if (Input.GetMouseButtonDown(0)) // Left-click
//         {
//             Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             mouseWorldPos.z = 0; // Important for 2D

//             var node = gridGraph.GetNodeFromWorld(mouseWorldPos);

//             if (node != null)
//             {
//                 Debug.Log($"Clicked tile at grid ({node.x}, {node.y}), walkable: {node.walkable}");
//             }
//             else
//             {
//                 Debug.Log("Clicked outside the grid bounds.");
//             }
//         }
//     }
// }
