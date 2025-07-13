using UnityEngine;

public class Base : MonoBehaviour, IProjectilePattern
{
    public void ProjectileOnCollision(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}