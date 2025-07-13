using UnityEngine;

public class SplashBurst : MonoBehaviour, IProjectilePattern
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private int _projectileCount = 5;
    [SerializeField] private float _spreadAngle = 360f;
    [SerializeField] private float _damageMultiplier = 0.5f;
    [SerializeField] private float _speedMultiplier = 1f;
    [SerializeField] private float _lifetime = 3f;

    public void ProjectileOnCollision(GameObject gameObject)
    {
        if (!gameObject.TryGetComponent<Projectile>(out var parentProjectile)) return;

        Vector3 origin = parentProjectile.transform.position;
        Transform parentTransform = parentProjectile.transform.parent;

        for (int i = 0; i < _projectileCount; i++)
        {
            float angle = 360f / _projectileCount * i;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            GameObject newProj = Object.Instantiate(_projectilePrefab, origin, Quaternion.identity, parentTransform);
            if (newProj.TryGetComponent<Projectile>(out var proj))
            {
                proj.Init(
                    direction,
                    parentProjectile.GetDamage() * _damageMultiplier,
                    parentProjectile.GetSpeed() * _speedMultiplier,
                    _lifetime
                );
            }
        }

        Object.Destroy(gameObject);
    }
}
