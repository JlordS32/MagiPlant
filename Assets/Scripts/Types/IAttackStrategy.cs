using UnityEngine;

public interface IAttackStrategy
{
    void Attack(Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent);
}

