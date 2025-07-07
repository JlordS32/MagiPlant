using UnityEngine;

public class EnemyAttack : MonoBehaviour, IAttack
{
    [SerializeField] float _coolDown;
    [SerializeField] float _damage = 5f;

    // REFERENCES
    EnemyAnimation _enemyAnim;

    public float Cooldown => _coolDown;

    void Awake()
    {
        _enemyAnim = GetComponent<EnemyAnimation>();
    }

    public void Attack(IDamageable target)
    {
        _enemyAnim.AnimateJump(((MonoBehaviour)target).transform);
        target.TakeDamage(_damage);
    }
}
