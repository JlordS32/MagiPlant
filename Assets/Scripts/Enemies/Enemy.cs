using UnityEngine;

public class Enemy : MonoBehaviour, IAttack, IDamageable
{
    // PROPERTIES
    [SerializeField] float _coolDown;
    [SerializeField] float _damage = 5f;
    [SerializeField] EnemyStatConfig _enemyStatConfig;

    // REFERENCES
    EnemyAnimation _enemyAnim;
    Rigidbody2D _rb;
    EnemyData _enemyData;
    HealthUI _healthUI;

    // VARIABLES
    float _timer;
    IDamageable _targetInRange;

    void Awake()
    {
        _enemyAnim = GetComponent<EnemyAnimation>();
        _rb = GetComponent<Rigidbody2D>();
        _enemyData = new EnemyData(_enemyStatConfig);
        _healthUI = GetComponentInChildren<HealthUI>();
    }

    void OnEnable() => EnemyManager.Register(this);
    void OnDisable() => EnemyManager.Unregister(this);

    void Update()
    {
        if (_enemyData.IsDead) return;

        _timer += Time.deltaTime;

        // Flip sprite based on direction
        if (_rb.linearVelocityX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Sign(_rb.linearVelocityX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

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

    public void TakeDamage(float dmg)
    {
        _healthUI.Show();
        _enemyData.ApplyDamage(dmg);
        _healthUI.UpdateBar(_enemyData.Get(EnemyStats.HP), _enemyData.Get(EnemyStats.MaxHP));

        if (_enemyData.IsDead)
        {
            Destroy(gameObject);
        }
    }
}
