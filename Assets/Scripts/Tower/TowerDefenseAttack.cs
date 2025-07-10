using UnityEngine;

[System.Serializable]
public struct ProjectileStats
{
    public float damage;
    public float speed;
    public float lifetime;
}

public class TowerDefenseAttack : MonoBehaviour
{
    [SerializeField] Transform _firePoint;
    [SerializeField] Transform _projectileParent;
    [SerializeField] float _coolDown = 1f;
    [SerializeField] ScriptableObject _attackStrategyObject;

    IAttackStrategy _attackStrategy;
    float _timer;

    void Awake()
    {
        _attackStrategy = _attackStrategyObject as IAttackStrategy;
    }

    void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Shoot(Vector3 direction, ProjectileStats stats)
    {
        if (_timer >= _coolDown && _attackStrategy != null)
        {
            _attackStrategy.Attack(direction, stats, _firePoint, _projectileParent);
            _timer = 0f;
        }
    }
}
