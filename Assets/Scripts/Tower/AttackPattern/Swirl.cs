using UnityEngine;

// TODO[DUY]: Document this code, dowee.
public class Swirl : MonoBehaviour, IAttackStrategy
{
    [SerializeField, Range(3, 36)] int _projectileCount = 8;
    [SerializeField] float _swirlOffsetAngle = 10f;

    float _currentSwirlAngle = 0f;

    public void Attack(GameObject projectilePrefab, Vector3 direction, ProjectileStats stats, Transform firePoint, Transform parent)
    {
        float angleStep = 360f / _projectileCount;

        for (int i = 0; i < _projectileCount; i++)
        {
            float angle = _currentSwirlAngle + angleStep * i;
            Vector3 dir = AngleToDirection(angle);

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity, parent);
            proj.GetComponent<Projectile>().Init(dir, stats);
        }

        _currentSwirlAngle += _swirlOffsetAngle;
    }

    private Vector3 AngleToDirection(float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f).normalized;
    }
}
