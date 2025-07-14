using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Properties")]
    [SerializeField] int _maxSpawn = 10;

    // VARIABLES
    float _timer;

    // REFERENCES
    TileManager _tileManager;

    void Awake()
    {
        _tileManager = FindFirstObjectByType<TileManager>();
        
        // Snap spawner to tile.
        Vector3Int cell = _tileManager.Map.WorldToCell(transform.position);
        Vector3 center = _tileManager.Map.GetCellCenterWorld(cell);
        transform.position = center;
    }

    public bool TrySpawn(GameObject prefab, Vector3 offset = default)
    {
        if (transform.childCount >= _maxSpawn) return false;

        Instantiate(prefab, transform.position + offset,
                    Quaternion.identity, transform);
        return true;
    }
}
