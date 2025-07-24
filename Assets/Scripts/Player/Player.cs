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

    void Awake()
    {
        _playerData = new PlayerData(_statConfig);
        _healthUI = GetComponent<HealthUI>();
    }

    void Start()
    {
        // Take a spot on the grid on initial load.
        Vector3Int gridIndex = TileManager.Instance.Map.WorldToCell(transform.position);
        TileManager.Instance.SetOccupiedArea(gridIndex, _tileSize.x, _tileSize.y, TileWeight.Walkable);
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
        _playerData.LogAllStats();
    }
}
