using UnityEngine;

public class ResourceGenerator : MonoBehaviour, IBuildable
{
    public CurrencyType ResourceType = CurrencyType.Sunlight;
    public float GenerateAmount = 1f;
    public float Interval = 2f;

    private float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= Interval)
        {
            CurrencyStorage.Instance.Add(ResourceType, GenerateAmount);
            _timer = 0f;
        }
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
