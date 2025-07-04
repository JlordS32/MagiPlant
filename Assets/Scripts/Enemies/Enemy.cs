using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform _target;
    [SerializeField] bool _enableDebug;

    [Header("Enemy Properties")]
    [SerializeField] float _speed = 2f;
    [SerializeField] int _stoppingDistance;

    // VARIABLES
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
        Queue<Vector3> newPath = new();

        // Convert start and goal pos to tilemap cells
        Vector3Int startCell = _tileManager.Map.WorldToCell(transform.position);
        Vector2 to_Target = (transform.position - _target.position).normalized;
        Vector3 off_Target = _target.position + (Vector3)(to_Target * _stoppingDistance);
        Vector3Int goalCell = _tileManager.Map.WorldToCell(off_Target);

        // Normalize to grid indices
        Vector2Int start = new(startCell.x - _tileManager.Bounds.xMin, startCell.y - _tileManager.Bounds.yMin);
        Vector2Int goal = new(goalCell.x - _tileManager.Bounds.xMin, goalCell.y - _tileManager.Bounds.yMin);

        var path = AStar.FindPath(start, goal, _tileManager.GetGridAsInt());
        if (path == null) return;

        // Convert grid path to world positions
        foreach (var step in path)
        {
            Vector3Int cell = new(step.x + _tileManager.Bounds.xMin, step.y + _tileManager.Bounds.yMin, 0);
            Vector3 world = _tileManager.Map.CellToWorld(cell) + _tileManager.Map.cellSize / 2f;

            // Add noise for randomness
            world += new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0f
            );

            newPath.Enqueue(world);
        }

        _path = newPath;
    }

    void Update()
    {
        // FEATURE: Repathing logic here, for now disregard
        // not needed for the current game logic.

        // Pathing logic
        if (_path.Count == 0) return;

        Vector3 _targetPos = _path.Peek();
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPos) < 0.05f)
        {
            _path.Dequeue();
        }
    }

    IEnumerator DelayedPathRequest()
    {
        yield return new WaitForSeconds(1);
        RequestPath();
    }

    void OnDrawGizmos()
    {
        if (_path == null || _path.Count == 0 || !_enableDebug)
            return;

        Gizmos.color = Color.cyan;

        Vector3[] points = _path.ToArray();
        Vector3 previous = transform.position;

        // Draw path taken
        foreach (var point in points)
        {
            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.1f);
            previous = point;
        }

        // Draw stopping zone
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(_target.position, _stoppingDistance);
    }
}
