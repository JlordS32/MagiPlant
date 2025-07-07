using UnityEngine;

[System.Serializable]
public struct ProjectileStats
{
    public float damage;
    public float speed;
    public float lifetime;
}

public class TowerDefenseAttack : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _projectTileParent;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _coolDown = 1f;

    [Header("Project Properties")]
    [SerializeField] ProjectileStats _projectileStats;

    float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Shoot(Vector3 direction)
    {
        if (_timer >= _coolDown)
        {
            GameObject proj = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity, _projectTileParent);
            proj.GetComponent<Projectile>().Init(direction.normalized, _projectileStats.damage, _projectileStats.speed, _projectileStats.lifetime);
            _timer = 0;
        }
    }
}
