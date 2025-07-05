using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Transform _target;
    [SerializeField] bool _enableDebug;
    [SerializeField] float _pathFindingDelay = 1f;

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

        // Convert world to grid
        Vector3Int startCell = _tileManager.Map.WorldToCell(transform.position);
        Vector3Int targetCell = _tileManager.Map.WorldToCell(_target.position);

        Vector2Int start = new(startCell.x - _tileManager.Bounds.xMin, startCell.y - _tileManager.Bounds.yMin);
        Vector2Int target = new(targetCell.x - _tileManager.Bounds.xMin, targetCell.y - _tileManager.Bounds.yMin);

        // Find nearest walkable tile around the target
        Vector2Int goal = target;
        List<Vector2Int> possibleDir = new();

        foreach (Vector2Int dir in AStar.directions)
        {
            Vector2Int check = target + dir;
            if (_tileManager.IsInBounds(check.x, check.y) && _tileManager.Grid[check.x, check.y] > 0)
            {
                possibleDir.Add(check);
            }
        }

        if (possibleDir.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name} couldn't find walkable goal near target.");
            return;
        }
        else
        {
            goal = possibleDir[Random.Range(0, possibleDir.Count)];
        }

        // Run A*
        var path = AStar.FindPath(start, goal, _tileManager.GetGridAsInt());
        if (path == null)
        {
            Debug.LogWarning($"{gameObject.name} failed to find path. Start: {start}, Goal: {goal}");
            return;
        }

        // Convert grid path to world
        foreach (var step in path)
        {
            Vector3Int cell = new(step.x + _tileManager.Bounds.xMin, step.y + _tileManager.Bounds.yMin, 0);
            Vector3 world = _tileManager.Map.CellToWorld(cell) + _tileManager.Map.cellSize / 2f;

            // Optional: add slight offset
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
        yield return new WaitForSeconds(_pathFindingDelay);
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
