using System.Linq;
using UnityEngine;

public class TowerDetection : MonoBehaviour
{
    [SerializeField] Vector3 _localForward = Vector3.down;

    [Header("Debug Mode")]
    [SerializeField] bool _enableDebug;

    // REFERENCES
    float _range;
    TowerDefense _towerDefense;

    void Awake()
    {
        _towerDefense = GetComponent<TowerDefense>();
    }

    void Start()
    {
        _range = _towerDefense.Data.Get(TowerStats.Range);
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

    public Enemy GetClosestEnemyInRange()
    {
        return EnemyManager.Enemies
            .Where(e => Vector3.Distance(e.transform.position, transform.position) <= _range)
            .OrderBy(e => Vector3.Distance(e.transform.position, transform.position))
            .FirstOrDefault();
    }
}
