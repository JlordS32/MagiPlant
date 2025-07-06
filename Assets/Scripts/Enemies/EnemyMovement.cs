using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] bool _enableDebug;
    [SerializeField] float _pathFindingDelay = 1f;

    [Header("Enemy Properties")]
    [SerializeField] float _speed = 2f;
    [SerializeField] int _stoppingDistance;

    // VARIABLES
    TileManager _tileManager;
    Rigidbody2D _rb;
    Transform _target;
    Queue<Vector3> _path = new();

    void Awake()
    {
        _tileManager = FindFirstObjectByType<TileManager>();
        _rb = GetComponent<Rigidbody2D>();
        _target = FindFirstObjectByType<Player>().GetComponent<Transform>();
    }

    void Start()
    {
        StartCoroutine(DelayedPathRequest());
    }

    // BUG: Enemy target cell is not aligned. 
    // Suggested fix: Look at BuildManager.s, the alignment must be dynamic.
    // Other suggestions: Fix tile size on the tree instance.
    void RequestPath()
    {
        Queue<Vector3> newPath = new();

        // Convert world to grid
        Vector3Int startCell = _tileManager.Map.WorldToCell(transform.position);
        Vector3Int targetCell = _tileManager.Map.WorldToCell(_target.position);

        Vector2Int start = new(startCell.x - _tileManager.Bounds.xMin, startCell.y - _tileManager.Bounds.yMin);
        Vector2Int target = new(targetCell.x - _tileManager.Bounds.xMin, targetCell.y - _tileManager.Bounds.yMin);

        // Find nearest walkable tile around the target
        List<Vector2Int> possibleDir = new();
        foreach (Vector2Int dir in AStar.directions)
        {
            Vector2Int check = target + dir;
            if (_tileManager.IsInBounds(check.x, check.y) && _tileManager.Grid[check.x, check.y] > 0)
            {
                possibleDir.Add(check);
            }
        }

        Vector2Int goal;
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
        Vector2 direction = (_targetPos - transform.position).normalized;
        _rb.linearVelocity = direction * _speed;

        if (Vector3.Distance(transform.position, _targetPos) < 0.05f)
        {
            _path.Dequeue();
        }

        if (_path.Count == 0)
            _rb.linearVelocity = Vector2.zero;
    }

    IEnumerator DelayedPathRequest()
    {
        yield return new WaitForSeconds(_pathFindingDelay);
        RequestPath();
    }

    void OnDrawGizmos()
    {
        if (_tileManager == null || !_enableDebug) return;

        // Draw path lines
        if (_path != null && _path.Count > 0)
        {
            Gizmos.color = Color.cyan;

            Vector3[] points = _path.ToArray();
            Vector3 previous = transform.position;

            foreach (var point in points)
            {
                Gizmos.DrawLine(previous, point);
                previous = point;
            }
        }

        // Draw shaded target tile
        if (_target != null)
        {
            Vector3 targetPos = _target.position;
            Vector3Int cell = _tileManager.Map.WorldToCell(targetPos);
            Vector3 cellCenter = _tileManager.Map.GetCellCenterWorld(cell);
            Vector3 cellSize = _tileManager.Map.cellSize;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawCube(cellCenter, cellSize);
        }
    }

}
