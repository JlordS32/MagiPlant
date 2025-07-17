using UnityEngine;

public class Projectile : MonoBehaviour, IAttack
{
    IProjectilePattern _projectilePattern;

    Vector3 _direction;
    ProjectileStats _stats;
    

    void Awake()
    {
        _projectilePattern = GetComponent<IProjectilePattern>();

        // Fallback to a default projectile pattern
        if (_projectilePattern == null)
        {
            _projectilePattern = gameObject.AddComponent<Base>();
        }
    }

    public void Init(Vector3 direction, ProjectileStats stats)
    {
        _direction = direction.normalized;
        _stats = stats;
        Destroy(gameObject, _stats.lifetime);
    }

    void Update()
    {
        transform.position += Time.deltaTime * _stats.speed * _direction;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var target) && collision.CompareTag("Enemies"))
        {
            Attack(target);

            // projectile pattern when it hit the enemy
            _projectilePattern?.ProjectileOnCollision(gameObject);
        }
    }

    public void Attack(IDamageable target)
    {
        target.TakeDamage(_stats.damage);
    }

    public float GetDamage() => _stats.damage;
    public float GetSpeed() => _stats.speed;
}
