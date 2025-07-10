using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/AttackStrategies/DoubleShot")]
public class DoubleShotAttack : ScriptableObject, IAttackStrategy
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] float _spreadAngle = 10f; // angle between the two shots in degrees

    public void Attack(Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent)
    {
        direction.Normalize();

        // Compute spread directions
        Vector3 leftDir = Quaternion.Euler(0, 0, -_spreadAngle / 2f) * direction;
        Vector3 rightDir = Quaternion.Euler(0, 0, _spreadAngle / 2f) * direction;

        // Instantiate left projectile
        GameObject leftProj = Instantiate(_projectilePrefab, firePoint.position, Quaternion.identity, parent);
        leftProj.GetComponent<Projectile>().Init(leftDir, stats.damage, stats.speed, stats.lifetime);

        // Instantiate right projectile
        GameObject rightProj = Instantiate(_projectilePrefab, firePoint.position, Quaternion.identity, parent);
        rightProj.GetComponent<Projectile>().Init(rightDir, stats.damage, stats.speed, stats.lifetime);
    }
}
