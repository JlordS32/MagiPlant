using UnityEngine;

public class SingleTarget : MonoBehaviour, IAttackStrategy
{
    public void Attack(GameObject projectilePrefab, Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity, parent);
        proj.GetComponent<Projectile>().Init(direction, stats.damage, stats.speed, stats.lifetime);
    }
}

