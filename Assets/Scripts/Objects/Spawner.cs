using UnityEngine;

public class Spawner : MonoBehaviour
{
    void Awake()
    {   
        // Snap spawner to tile.
        Vector3Int cell = TileManager.Instance.Map.WorldToCell(transform.position);
        Vector3 center = TileManager.Instance.Map.GetCellCenterWorld(cell);
        transform.position = center;
    }

    public bool TrySpawn(GameObject prefab, Vector3 offset = default)
    {
        Instantiate(prefab, transform.position + offset,
                    Quaternion.identity, transform);
        return true;
    }
}
