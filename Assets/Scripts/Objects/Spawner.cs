using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Properties")]
    [SerializeField] int _maxSpawn = 10;

    // VARIABLES
    float _timer;

    void Awake()
    {   
        // Snap spawner to tile.
        Vector3Int cell = TileManager.Instance.Map.WorldToCell(transform.position);
        Vector3 center = TileManager.Instance.Map.GetCellCenterWorld(cell);
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
