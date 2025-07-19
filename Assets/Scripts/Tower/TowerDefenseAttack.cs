using UnityEngine;

public class TowerDefenseAttack : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _firePoint;

    IAttackStrategy _attackStrategy;
    TowerDefense _tower;
    TowerData _towerData;
    float _timer;
    float _speedFactor = 100;

// Ensure the projectile prefab has a Projectile component
#if UNITY_EDITOR
    void OnValidate()
    {
        if (_projectilePrefab &&
            _projectilePrefab.GetComponent<Projectile>() == null)
        {
            Debug.LogWarning("Projectile prefab does not have a Projectile component assigned on " + gameObject.name);
            _projectilePrefab = null;
        }
    }
#endif

    void Awake()
    {
        _attackStrategy = GetComponent<IAttackStrategy>();
        _tower = GetComponent<TowerDefense>();
        _towerData = _tower.Data;

        // Fallback to a default attack strategy if none is set
        if (_attackStrategy == null)
        {
            _attackStrategy = gameObject.AddComponent<SingleTarget>();
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Shoot(Vector3 direction)
    {
        ProjectileStats projStats = new()
        {
            damage = _towerData.Get(TowerStats.Attack),
            speed = _towerData.Get(TowerStats.ProjectileSpeed),
            lifetime = _towerData.Get(TowerStats.ProjectileLifetime)
        };

        if (_timer >= _speedFactor / _towerData.Get(TowerStats.Speed) && _attackStrategy != null)
        {
            _attackStrategy.Attack(_projectilePrefab, direction, projStats, _firePoint, transform);
            _timer = 0f;
        }
    }
}
