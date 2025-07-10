using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/AttackStrategies/SingleProjectile")]
public class SingleProjectileAttack : ScriptableObject, IAttackStrategy
{
    [SerializeField] GameObject _projectilePrefab;

    public void Attack(Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent)
    {
        GameObject proj = Instantiate(_projectilePrefab, firePoint.position, Quaternion.identity, parent);
        proj.GetComponent<Projectile>().Init(direction, stats.damage, stats.speed, stats.lifetime);
    }
}
