using UnityEngine;

public class TowerDefense : MonoBehaviour, IBuildable, IUpgradeable
{
    [SerializeField] TowerStatConfig _towerStatConfig;

    // REFERENCES
    TowerData _towerData;
    TowerDefenseAttack _towerAttack;
    TowerDetection _towerDetection;

    // GETTERS
    public TowerData Data => _towerData;
    public TowerStatConfig StatConfig => _towerStatConfig;

    void Awake()
    {
        _towerAttack = GetComponent<TowerDefenseAttack>();
        _towerData = new TowerData(_towerStatConfig);
        _towerDetection = GetComponent<TowerDetection>();
    }

    void Update()
    {
        var target = _towerDetection.GetClosestEnemyInRange();
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;

        // Comment if want to prevent rotate
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // float forwardAngle = Mathf.Atan2(_localForward.y, _localForward.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.AngleAxis(angle - forwardAngle, Vector3.forward);

        _towerAttack.Shoot(dir);
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
        _towerData.TryUpgradeAll();
    }

    public IStatData GetData() => _towerData;
    [ContextMenu("Log Tower Stats")]
    void DebugStats()
    {
        _towerData.LogAllStats();
    }
}
