using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] List<GameObject> _enemyPrefab;
    [SerializeField] Vector2 _range;
    [SerializeField] float _interval;

    // VARIABLES
    float _timer;

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

        int random = Random.Range(0, _enemyPrefab.Count - 1);
        Vector3 offset = new(
            Random.Range(-_range.x / 2f, _range.x / 2f),
            Random.Range(-_range.y / 2f, _range.y / 2f),
            0f
        );

        Instantiate(_enemyPrefab[random], transform.position + offset, Quaternion.identity, transform);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _range);
    }
}
