using UnityEngine;

public class DefenseAttack : MonoBehaviour
{
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] Transform _projectTileParent;
    [SerializeField] Transform _firePoint;
    [SerializeField] float _coolDown = 1f;

    float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
    }

    public void Attack(Vector3 direction)
    {
        if (_timer >= _coolDown)
        {
            GameObject proj = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity, _projectTileParent);
            proj.GetComponent<Projectile>().Init(direction.normalized);
            _timer = 0;
        }
    }
}
