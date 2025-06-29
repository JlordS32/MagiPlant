using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 2f;
    [SerializeField] bool _enableDebug;

    // VARIABLES
    TileManager _tileManager;
    Queue<Vector3> _path = new();
    float _repathCooldown = 0.25f;
    float _repathTimer = 0;
    float _repathThreshold = 1.5f;
    int _pathStepCount = 0;
    int _pathHalfwayIndex = 0;
    int _stepsTaken = 0;

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

            // Add noise for randomness
            world += new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0f
            );

            newPath.Enqueue(world);
        }

        _path = newPath;
        _pathStepCount = _path.Count;
        _stepsTaken = 0;
        _pathHalfwayIndex = Mathf.Max(2, Mathf.FloorToInt(_pathStepCount * 0.5f));
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) <= _repathThreshold)
        {
            return;
        }

        // Path recalculation logic
        _repathTimer += Time.deltaTime;
        if (_stepsTaken >= _pathHalfwayIndex && _repathTimer >= _repathCooldown)
        {
            _repathTimer = 0f;
            RequestPath();
        }

        // Pathing logic
        if (_path.Count == 0) return;

        Vector3 targetPos = _path.Peek();
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            _path.Dequeue();
            _stepsTaken++;
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
    }
}
