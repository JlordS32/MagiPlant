using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _speed = 10f;
    [SerializeField] float _lifetime = 5f;

    Vector3 _direction;

    public void Init(Vector3 direction)
    {
        _direction = direction.normalized;
        Destroy(gameObject, _lifetime);
    }

    void Update()
    {
        transform.position += Time.deltaTime * _speed * _direction;
    }
}
