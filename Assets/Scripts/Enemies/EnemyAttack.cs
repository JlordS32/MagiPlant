using UnityEngine;

public class EnemyAttack : MonoBehaviour, IAttack
{
    [SerializeField] float _coolDown;
    [SerializeField] float _damage = 5f;

    // REFERENCES
    EnemyAnimation _enemyAnim;
    Enemy _enemy;
    float _attack;

    public float Cooldown => _coolDown;

    void Awake()
    {
        _enemyAnim = GetComponent<EnemyAnimation>();
        _enemy = GetComponent<Enemy>();
        _attack = _enemy.Data.Get(EnemyStats.Attack);
    }

    public void Attack(IDamageable target)
    {
        _enemyAnim.AnimateJump(((MonoBehaviour)target).transform);
        target.TakeDamage(_attack);
    }
}
