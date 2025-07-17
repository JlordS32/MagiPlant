using UnityEngine;

public class EnemyAttack : MonoBehaviour, IAttack
{
    [SerializeField] float _coolDown;

    // REFERENCES
    EnemyAnimation _enemyAnim;
    Enemy _enemy;
    EnemyData _enemyData;

    public float Cooldown => _coolDown;

    void Awake()
    {
        _enemyAnim = GetComponent<EnemyAnimation>();
        _enemy = GetComponent<Enemy>();
        _enemyData = _enemy.Data;
    }

    public void Attack(IDamageable target)
    {
        _enemyAnim.AnimateJump(((MonoBehaviour)target).transform);
        target.TakeDamage(_enemyData.Get(EnemyStats.Attack));
    }
}
