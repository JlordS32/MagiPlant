using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    // Player stat configurations
    [SerializeField] PlayerStatConfig _statConfig;
    [SerializeField] Vector2Int _tileSize;

    // VARIABLES
    PlayerData _playerData;
    public PlayerData PlayerData => _playerData;
    HealthUI _healthUI;
    TileManager _tileManager;

    void Awake()
    {
        _playerData = new PlayerData(_statConfig);
        _healthUI = GetComponent<HealthUI>();
        _tileManager = FindFirstObjectByType<TileManager>();
    }

    void Start()
    {
        // Take a spot on the grid on initial load.
        Vector3Int gridIndex = _tileManager.Map.WorldToCell(transform.position);
        _tileManager.SetOccupiedArea(gridIndex, _tileSize.x, _tileSize.y, TileWeight.Walkable);
    }

    public void TakeDamage(float dmg)
    {
        _healthUI.Show();
        _playerData.ApplyDamage(dmg);
        _healthUI.UpdateBar(_playerData.Get(PlayerStats.HP), _playerData.Get(PlayerStats.MaxHP));
    }

    [ContextMenu("Log Player Stats")]
    void DebugStats()
    {
        foreach (var kv in _playerData.Snapshot)
            Debug.Log($"{kv.Key}: {kv.Value}");
    }
}
