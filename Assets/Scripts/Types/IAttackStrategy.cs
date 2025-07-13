using UnityEngine;

public interface IAttackStrategy
{
    void Attack(GameObject projectilePrefab, Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent);
}

