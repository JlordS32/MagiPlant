using UnityEngine;
using System;

[Serializable]
public struct ProjectileStats
{
    public float damage;
    public float speed;
    public float lifetime;
}

public class TowerDefenseAttack : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _coolDown = 1f;

    IAttackStrategy _attackStrategy;
    float _timer;

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

    public void Shoot(Vector3 direction, ProjectileStats stats)
    {
        if (_timer >= _coolDown && _attackStrategy != null)
        {
            _attackStrategy.Attack(_projectilePrefab, direction, stats, _firePoint, transform);
            _timer = 0f;
        }
    }
}
