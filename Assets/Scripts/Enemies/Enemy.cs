using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    // PROPERTIES
    [SerializeField] EnemyStatConfig _enemyStatConfig;

    // REFERENCES
    EnemyData _enemyData;
    EnemyAttack _enemyAttack;
    HealthUI _healthUI;

    // VARIABLES
    IDamageable _targetInRange;
    float _timer;

    public EnemyData Data => _enemyData;

    void Awake()
    {
        _enemyAttack = GetComponent<EnemyAttack>();
        _enemyData = new EnemyData(_enemyStatConfig);
        _healthUI = GetComponentInChildren<HealthUI>();

        Debugger.Log(DebugCategory.Enemies, $"Initialized: {gameObject.name} with HP {_enemyData.Get(EnemyStats.HP)}");
    }

    void OnEnable() => EnemyManager.Instance.Register(this);
    void OnDisable() => EnemyManager.Instance.Unregister(this);

    void Update()
    {
        if (_enemyData.IsDead) return;
        _timer += Time.deltaTime;

        if (_targetInRange != null && _timer >= _enemyAttack.Cooldown)
        {
            _enemyAttack.Attack(_targetInRange);
            _timer = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<IDamageable>(out var target))
        {
            _targetInRange = target;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            _targetInRange = null;
    }

    public void TakeDamage(float dmg)
    {
        _healthUI.Show();
        _enemyData.ApplyDamage(dmg);
        _healthUI.UpdateBar(_enemyData.Get(EnemyStats.HP), _enemyData.Get(EnemyStats.MaxHP));

        if (_enemyData.IsDead)
        {
            Debugger.Log(DebugCategory.Enemies, $"Enemy defeated: {gameObject.name}");
            Destroy(gameObject);
        }
    }

    [ContextMenu("Log Enemy Stats")]
    void DebugStats()
    {
        _enemyData.LogAllStats();
    }
}
