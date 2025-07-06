using UnityEngine;

// TODO: Enemy Attacks Player
// TODO: Enemy Takes Damage
// TODO: Bouncing animation when attacking.
public class Enemy : MonoBehaviour, IAttack
{
    // PROPERTIES
    [SerializeField] float _coolDown;
    [SerializeField] float _damage = 5f;

    // REFERENCES
    EnemyAnimation _enemyAnim;

    // VARIABLES
    float _timer;

    void Awake()
    {
        _enemyAnim = GetComponent<EnemyAnimation>();
    }

    void OnEnable() => EnemyManager.Register(this);
    void OnDisable() => EnemyManager.Unregister(this);

    IDamageable _targetInRange;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_targetInRange != null && _timer >= _coolDown)
        {
            Attack(_targetInRange);
            _timer = 0f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<IDamageable>(out var target))
            _targetInRange = target;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _targetInRange = null;
    }

    public void Attack(IDamageable target)
    {
        _enemyAnim.AnimateJump(((MonoBehaviour)target).transform);
        target.TakeDamage(_damage);
    }
}
