using UnityEngine;

public class Projectile : MonoBehaviour, IAttack
{
    IProjectilePattern _projectilePattern;

    float _damage;
    Vector3 _direction;
    float _speed;

    void Awake()
    {
        _projectilePattern = GetComponent<IProjectilePattern>();

        // Fallback to a default projectile pattern
        if (_projectilePattern == null)
        {
            _projectilePattern = gameObject.AddComponent<Base>();
        }
    }

    public void Init(Vector3 direction, float damage = 5f, float speed = 10f, float lifetime = 5f)
    {
        _direction = direction.normalized;
        _damage = damage;
        _speed = speed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Time.deltaTime * _speed * _direction;
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
        target.TakeDamage(_damage);
    }

    public float GetDamage() => _damage;
    public float GetSpeed() => _speed;
}
