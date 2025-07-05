using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
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
        Debug.Log("Player taking damage");
        _playerData.ApplyDamage(damage);
    }
}
