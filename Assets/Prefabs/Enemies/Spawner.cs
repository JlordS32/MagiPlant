using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<GameObject> _enemyPrefab;
    [SerializeField] Vector2 _range;
    [SerializeField] float _interval;

    // VARIABLES
    float _timer;

    // REFERENCES
    TileManager _tileManager;

    void Awake()
    {
        _tileManager = FindFirstObjectByType<TileManager>();
    }

    void Update()
    {

        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            _timer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (_enemyPrefab.Count == 0) return;

        int random = Random.Range(0, _enemyPrefab.Count);
        Vector3Int baseCell = _tileManager.Map.WorldToCell(transform.position);

        // Try random nearby cells
        while (true)
        {
            Vector3Int offset = new(
                Random.Range(-Mathf.FloorToInt(_range.x / 2), Mathf.CeilToInt(_range.x / 2)),
                Random.Range(-Mathf.FloorToInt(_range.y / 2), Mathf.CeilToInt(_range.y / 2)),
                0
            );

            Vector3Int cell = baseCell + offset;
            Vector2Int grid = _tileManager.TileToGrid(cell);

            if (_tileManager.IsInBounds(grid.x, grid.y) &&
                _tileManager.Grid[grid.x, grid.y] > 0)
            {
                Vector3 world = _tileManager.Map.CellToWorld(cell) + _tileManager.Map.cellSize / 2f;
                Instantiate(_enemyPrefab[random], world, Quaternion.identity, transform);
                return;
            }
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _range);
    }
}
