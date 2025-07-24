using UnityEngine;

public class ResourceBuilding : MonoBehaviour, IBuildable, IUpgradeable
{
    // PROPERTIES
    [SerializeField] ResourceStatConfig _config;
    ResourceData _resourceData;

    // TODO[JAYLOU]: ADD SUPPORT FOR GENERATION, make it into an upgrade. So it's an auto-clicker.

    public ResourceData Data => _resourceData;

    void Awake()
    {
        _resourceData = new ResourceData(_config);
    }

    void OnMouseUpAsButton()
    {
        foreach (var currency in _resourceData.Snapshot)
        {
            CurrencyStorage.Instance.Add(currency.Key, currency.Value);
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

    public void Upgrade()
    {
        _resourceData.UpgradeAll();
    }

    public IStatData GetData() => _resourceData;
    [ContextMenu("Log Resource Stats")]
    void DebugStats()
    {
        _resourceData.LogAllStats();
    }
}
