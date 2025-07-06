using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    // UI
    [SerializeField] Image _healthBar;
    [SerializeField] GameObject _health;

    [SerializeField] PlayerStatConfig _statConfig;

    // VARIABLES
    PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    void Awake()
    {
        _playerData = new PlayerData(_statConfig);
    }

    public void TakeDamage(float damage)
    {
        if (_health != null && !_health.activeSelf)
        {
            _health.SetActive(true);
        }

        _playerData.ApplyDamage(damage);
        _healthBar.fillAmount = _playerData.Get(PlayerStats.HP) / _playerData.Get(PlayerStats.MaxHP);
    }
}
