using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    // Player stat configurations
    [SerializeField] PlayerStatConfig _statConfig;

    // VARIABLES
    PlayerData _playerData;
    public PlayerData PlayerData => _playerData;
    HealthUI _healthUI;

    void Awake()
    {
        _playerData = new PlayerData(_statConfig);
        _healthUI = GetComponent<HealthUI>();
    }

    public void TakeDamage(float dmg)
    {
        Debug.Log(dmg);
        _healthUI.Show();
        _playerData.ApplyDamage(dmg);
        _healthUI.UpdateBar(_playerData.Get(PlayerStats.HP), _playerData.Get(PlayerStats.MaxHP));
    }
}
