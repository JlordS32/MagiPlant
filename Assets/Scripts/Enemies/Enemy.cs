using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 2f;

    TileManager _tileManager;
    Queue<Vector3> _path = new();

    void Awake()
    {
        _tileManager = FindFirstObjectByType<TileManager>();
    }

    void Start()
    {
        StartCoroutine(DelayedPathRequest());
    }

    void RequestPath()
    {
        // Convert world to cell
        Vector3Int startCell = _tileManager.Map.WorldToCell(transform.position);
        Vector3Int goalCell = _tileManager.Map.WorldToCell(target.position);

        // Normalize to grid indices
        Vector2Int start = new(startCell.x - _tileManager.Bounds.xMin, startCell.y - _tileManager.Bounds.yMin);
        Vector2Int goal = new(goalCell.x - _tileManager.Bounds.xMin, goalCell.y - _tileManager.Bounds.yMin);

        var path = AStar.FindPath(start, goal, _tileManager.Grid);
        if (path == null) return;

        // Convert grid path to world positions
        foreach (var step in path)
        {
            Vector3Int cell = new(step.x + _tileManager.Bounds.xMin, step.y + _tileManager.Bounds.yMin, 0);
            Vector3 world = _tileManager.Map.CellToWorld(cell) + _tileManager.Map.cellSize / 2f;
            _path.Enqueue(world);
        }
    }

    void Update()
    {
        if (_path.Count == 0) return;

        Vector3 targetPos = _path.Peek();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
            _path.Dequeue();
    }

    IEnumerator DelayedPathRequest()
    {
        yield return new WaitForSeconds(1);
        RequestPath();
    }
}
