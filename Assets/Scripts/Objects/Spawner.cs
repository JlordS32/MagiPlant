using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Enemy to spawn")]
    [SerializeField] List<GameObject> _enemyPrefab;

    [Header("Spawn Properties")]
    [SerializeField] Vector2 _spawnRange;
    [SerializeField] float _spawnInterval;
    [SerializeField] int _maxSpawn = 10;

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

        if (_timer >= _spawnInterval)
        {
            _timer = 0;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (_enemyPrefab.Count == 0 || transform.childCount >= _maxSpawn) return;

        int random = Random.Range(0, _enemyPrefab.Count);
        Vector3Int baseCell = _tileManager.Map.WorldToCell(transform.position);

        // Try random nearby cells
        while (true)
        {
            Vector3Int offset = new(
                Random.Range(-Mathf.FloorToInt(_spawnRange.x / 2), Mathf.CeilToInt(_spawnRange.x / 2)),
                Random.Range(-Mathf.FloorToInt(_spawnRange.y / 2), Mathf.CeilToInt(_spawnRange.y / 2)),
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
        Gizmos.DrawWireCube(transform.position, _spawnRange);
    }
}
