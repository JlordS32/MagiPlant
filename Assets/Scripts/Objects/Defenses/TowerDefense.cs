using UnityEngine;

public class TowerDefense : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _localForward = Vector3.down;
    [SerializeField] float _strength = 5f;

    DefenseAttack _attack;

    void Awake()
    {
        _attack = GetComponent<DefenseAttack>();
    }

    void Update()
    {
        Vector3 dir = _target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float forwardAngle = Mathf.Atan2(_localForward.y, _localForward.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - forwardAngle, Vector3.forward);

        _attack.Attack(dir);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 worldDirection = transform.TransformDirection(_localForward.normalized);
        Gizmos.DrawLine(transform.position, transform.position + worldDirection * 2f);
    }
}
