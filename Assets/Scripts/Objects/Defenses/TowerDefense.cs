using System.Linq;
using UnityEngine;

// WARNING: Dowee this is for you. Add TowerData similar implementation as EnemyData
// WARNING: Add tower variants
public class TowerDefense : MonoBehaviour
{
    [SerializeField] Vector3 _localForward = Vector3.down;
    [SerializeField] float _range = 10f;

    [Header("Debug Mode")]
    [SerializeField] bool _enableDebug;

    TowerDefenseAttack _attack;

    void Awake()
    {
        _attack = GetComponent<TowerDefenseAttack>();
    }

    void Update()
    {
        var target = GetClosestEnemyInRange();
        if (target == null) return;

        Vector3 dir = target.transform.position - transform.position;

        // Comment if want to prevent rotate
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float forwardAngle = Mathf.Atan2(_localForward.y, _localForward.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - forwardAngle, Vector3.forward);

        _attack.Shoot(dir);
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
