using System.Linq;
using UnityEngine;

// WARNING: Dowee this is for you. Add TowerData similar implementation as EnemyData
// WARNING: Add tower variants
public class TowerDefense : MonoBehaviour
{
    [SerializeField] TowerStatConfig _towerStatConfig;
    [SerializeField] Vector3 _localForward = Vector3.down;
    [SerializeField] float _range = 10f;

    // REFERENCES
    TowerData _towerData;
    TowerDefenseAttack _towerAttack;

    [Header("Debug Mode")]
    [SerializeField] bool _enableDebug;


    void Awake()
    {
        _towerAttack = GetComponent<TowerDefenseAttack>();
        _towerData = new TowerData(_towerStatConfig);
    }

    void Update()
    {
        var target = GetClosestEnemyInRange();
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;

        // Comment if want to prevent rotate
        // float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // float forwardAngle = Mathf.Atan2(_localForward.y, _localForward.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.AngleAxis(angle - forwardAngle, Vector3.forward);

        ProjectileStats projStats = new ProjectileStats
        {
            damage = _towerData.Get(TowerStats.Attack),
            speed = _towerData.Get(TowerStats.Speed),
            lifetime = 5f
        };

        _towerAttack.Shoot(dir, projStats);
    }

    void OnDrawGizmos()
    {
        if (!_enableDebug) return;

        Gizmos.color = Color.red;
        Vector3 worldDirection = transform.TransformDirection(_localForward.normalized);
        Gizmos.DrawLine(transform.position, transform.position + worldDirection * 2f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    Enemy GetClosestEnemyInRange()
    {
        return EnemyManager.Enemies
            .Where(e => Vector3.Distance(e.transform.position, transform.position) <= _range)
            .OrderBy(e => Vector3.Distance(e.transform.position, transform.position))
            .FirstOrDefault();
    }

}
