using UnityEngine;

// TODO: Document this code, dowee.
public class DoubleShot : MonoBehaviour, IAttackStrategy
{
    [SerializeField] float _spreadAngle = 10f;

    public void Attack(GameObject projectilePrefab, Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent)
    {
        direction.Normalize();

        // Compute spread directions
        Vector3 leftDir = Quaternion.Euler(0, 0, -_spreadAngle / 2f) * direction;
        Vector3 rightDir = Quaternion.Euler(0, 0, _spreadAngle / 2f) * direction;

        // Instantiate left projectile
        GameObject leftProj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity, parent);
        leftProj.GetComponent<Projectile>().Init(leftDir, stats.damage, stats.speed, stats.lifetime);

        // Instantiate right projectile
        GameObject rightProj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity, parent);
        rightProj.GetComponent<Projectile>().Init(rightDir, stats.damage, stats.speed, stats.lifetime);
    }
}
