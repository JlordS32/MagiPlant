using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float speed = 2f;

    // VARIABLES
    TileManager _tileManager;
    Queue<Vector3> _path = new();
    Vector3 _lastTargetPos;
    float _repathThreshold = 0.5f;
    float _repathCooldown = 0.25f;
    float _repathTimer = 0;

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
        // Clean old path
        _path.Clear();

        // Convert world to cell
        Vector3Int startCell = _tileManager.Map.WorldToCell(transform.position);
        Vector3Int goalCell = _tileManager.Map.WorldToCell(target.position);

        // Normalize to grid indices
        Vector2Int start = new(startCell.x - _tileManager.Bounds.xMin, startCell.y - _tileManager.Bounds.yMin);
        Vector2Int goal = new(goalCell.x - _tileManager.Bounds.xMin, goalCell.y - _tileManager.Bounds.yMin);

        _tileManager.BuildGrid(); // Rebuild grid
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


            _path.Enqueue(world);
        }
    }

    void Update()
    {
        _repathTimer += Time.deltaTime;

        // If enemy is too far from player or timer has been reached
        if (Vector3.Distance(target.position, _lastTargetPos) > _repathThreshold && _repathTimer >= _repathCooldown)
        {
            _repathTimer = 0f;
            _lastTargetPos = target.position;
            RequestPath();
        }

        // Pathing logic
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

    void OnDrawGizmos()
    {
        if (_path == null || _path.Count == 0)
            return;

        Gizmos.color = Color.cyan;

        Vector3[] points = _path.ToArray();
        Vector3 previous = transform.position;

        foreach (var point in points)
        {
            Gizmos.DrawLine(previous, point);
            Gizmos.DrawSphere(point, 0.1f);
            previous = point;
        }
    }
}
