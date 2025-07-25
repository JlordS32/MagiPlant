using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] bool _enableDebug;
    [SerializeField] float _pathFindingDelay = 1f;

    [Header("Enemy Properties")]
    [SerializeField] int _stoppingDistance;

    [Range(0, 1)]
    [SerializeField] float wobbleX = 0.5f;

    [Range(0, 1)]
    [SerializeField] float wobbleY = 0.5f;

    // VARIABLES
    Enemy _enemy;
    Rigidbody2D _rb;
    Transform _target;
    Queue<Vector3> _path = new();
    
    float _speed;
    float _maxSpeed = 20f;
    float _minSpeed = 0.1f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = FindFirstObjectByType<Player>().GetComponent<Transform>();
        _enemy = GetComponent<Enemy>();
        _speed = Mathf.Clamp(_enemy.Data.Get(EnemyStats.Speed) / 10, _minSpeed, _maxSpeed);
    }

    void Start()
    {
        StartCoroutine(DelayedPathRequest());
    }

    void RequestPath()
    {
        Queue<Vector3> newPath = new();

        // Convert world to grid
        Vector3Int startCell = TileManager.Instance.Map.WorldToCell(transform.position);
        Vector3Int targetCell = TileManager.Instance.Map.WorldToCell(_target.position);

        Vector2Int start = new(startCell.x - TileManager.Instance.Bounds.xMin, startCell.y - TileManager.Instance.Bounds.yMin);
        Vector2Int target = new(targetCell.x - TileManager.Instance.Bounds.xMin, targetCell.y - TileManager.Instance.Bounds.yMin);

        if (!TileManager.Instance.IsInBounds(start.x, start.y) || !TileManager.Instance.IsInBounds(target.x, target.y))
        {
            Debugger.LogError(DebugCategory.Enemies, $"{_enemy.name} is out of bounds!!!");
            _path.Clear();
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // Run A*
        var path = AStar.FindPath(start, target, TileManager.Instance.GetGridAsInt());
        if (path == null || path.Count == 0)
        {
            _rb.linearVelocity = Vector2.zero;
            _path.Clear(); // Prevent old movement
            return;
        }

        // Convert grid path to world
        foreach (var step in path)
        {
            Vector3Int cell = new(step.x + TileManager.Instance.Bounds.xMin, step.y + TileManager.Instance.Bounds.yMin, 0);
            Vector3 world = TileManager.Instance.Map.CellToWorld(cell) + TileManager.Instance.Map.cellSize / 2f;

            // Optional: add slight offset
            world += new Vector3(
                Random.Range(-wobbleX, wobbleX),
                Random.Range(-wobbleY, wobbleY),
                0f
            );

            newPath.Enqueue(world);
        }

        _path = newPath;
    }

    void FixedUpdate()
    {
        // FEATURE: Repathing logic here, for now disregard
        // not needed for the current game logic.

        // Pathing logic
        if (_path.Count == 0) return;

        Vector3 _targetPos = _path.Peek();
        Vector2 direction = (_targetPos - transform.position).normalized;
        _rb.linearVelocity = _speed * direction;

        while (_path.Count > 0 && Vector3.Distance(transform.position, _path.Peek()) < 0.5f)
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
        if (TileManager.Instance == null || !_enableDebug) return;

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
            Vector3Int cell = TileManager.Instance.Map.WorldToCell(targetPos);
            Vector3 cellCenter = TileManager.Instance.Map.GetCellCenterWorld(cell);
            Vector3 cellSize = TileManager.Instance.Map.cellSize;

            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawCube(cellCenter, cellSize);
        }
    }

}
