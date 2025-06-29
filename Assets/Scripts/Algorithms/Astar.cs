using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    static readonly Vector2Int[] directions = {
        new(0, 1),   // up
        new(1, 0),   // right
        new(0, -1),  // down
        new(-1, 0),  // left
        new(1, 1),   // up-right
        new(-1, 1),  // up-left
        new(1, -1),  // down-right
        new(-1, -1)  // down-left
    };

    public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, int[,] grid)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int[,] gScore = new int[width, height];
        int[,] fScore = new int[width, height];
        Vector2Int[,] cameFrom = new Vector2Int[width, height];

        // Init scores
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                gScore[x, y] = int.MaxValue;
                fScore[x, y] = int.MaxValue;
            }


        // Base setup
        gScore[start.x, start.y] = 0;
        fScore[start.x, start.y] = Heuristic(start, goal);

        var openSet = new PriorityQueue<Vector2Int>();
        openSet.Enqueue(start, fScore[start.x, start.y]);

        while (openSet.Count > 0)
        {
            Vector2Int current = openSet.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (!InBounds(neighbor, width, height) || grid[neighbor.x, neighbor.y] <= 0) // Not walkable
                    continue;

                int tentativeG = gScore[current.x, current.y] + grid[neighbor.x, neighbor.y];
                if (tentativeG < gScore[neighbor.x, neighbor.y])
                {
                    gScore[neighbor.x, neighbor.y] = tentativeG;
                    fScore[neighbor.x, neighbor.y] = tentativeG + Heuristic(neighbor, goal);
                    cameFrom[neighbor.x, neighbor.y] = current;
                    openSet.Enqueue(neighbor, fScore[neighbor.x, neighbor.y]);
                }
            }
        }

        return null;
    }

    static List<Vector2Int> ReconstructPath(Vector2Int[,] cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new()
        {
            current
        };

        while (cameFrom[current.x, current.y] != Vector2Int.zero || current == Vector2Int.zero)
        {
            current = cameFrom[current.x, current.y];
            path.Add(current);
            if (current == Vector2Int.zero) break;
        }

        path.Reverse();

        return path;
    }

    // Euclidean
    static int Heuristic(Vector2Int a, Vector2Int b)
    {
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        return Mathf.RoundToInt(Mathf.Sqrt(dx * dx + dy * dy));
    }

    // Manhattan
    // static int Heuristic(Vector2Int a, Vector2Int b)
    // {
    //     return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    // }

    static bool InBounds(Vector2Int pos, int width, int height) =>
        pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
}
