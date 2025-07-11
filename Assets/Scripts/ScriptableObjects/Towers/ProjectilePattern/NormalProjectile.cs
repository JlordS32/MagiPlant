using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/ProjectilePattern/NormalProjectile")]

public class NormalProjectile : ScriptableObject, IProjectilePattern
{
    public void ProjectileOnCollision(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}