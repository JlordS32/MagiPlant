using UnityEngine;

public class ResourceBuilding : MonoBehaviour, IBuildable
{
    // PROPERTIES
    [SerializeField] ResourceStatConfig _config;

    // VARIABLES
    float _timer;

    void OnMouseUpAsButton()
    {
        // 
    }

    public Vector2Int GetFootprint()
    {
        var bounds = GetComponentInChildren<SpriteRenderer>().bounds.size;
        var cellSize = TileManager.Instance.Map.cellSize;
        return new Vector2Int(
            Mathf.CeilToInt(bounds.x / cellSize.x),
            Mathf.CeilToInt(bounds.y / cellSize.y));
    }
}
