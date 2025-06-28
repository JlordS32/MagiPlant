using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraDrag : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] float _dragSpeed = 2f;
    [SerializeField] float _inertiaDuration = 0.5f;
    [SerializeField] float _inertiaDamping = 5f;
    [SerializeField] Tilemap _grid;

    // VARIABLES
    Vector3 _dragOrigin;
    Vector3 _velocity;
    bool _isDragging = false;
    float _inertiaTime = 0;
    Bounds _worldBounds;

    void OnDrawGizmos()
    {
        if (_grid == null) return;

        // Recompute world bounds from local bounds
        _grid.CompressBounds();
        Bounds local = _grid.localBounds;
        Vector3 worldCenter = _grid.transform.TransformPoint(local.center);
        Vector3 worldSize = Vector3.Scale(local.size, _grid.transform.lossyScale);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(worldCenter, worldSize);
    }

    void Update()
    {
        _grid.CompressBounds();
        _worldBounds = _grid.localBounds;
        _worldBounds.center = _grid.transform.TransformPoint(_worldBounds.center);
        _worldBounds.size = Vector3.Scale(_worldBounds.size, _grid.transform.lossyScale);

        if (Input.GetMouseButtonDown(0))
        {
            _dragOrigin = Input.mousePosition;
            _isDragging = true;
            _velocity = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
            Vector3 move = new(-difference.x * _dragSpeed, -difference.y * _dragSpeed, 0);
            transform.Translate(move, Space.World);
            _velocity = move / Time.deltaTime;
            _dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _inertiaTime = 0;
        }

        if (!_isDragging && _velocity.magnitude > 0.01f)
        {
            _inertiaTime += Time.deltaTime;
            float t = 1 - Mathf.Clamp01(_inertiaTime / _inertiaDuration);
            Vector3 move = _velocity * t * Time.deltaTime;
            transform.Translate(move, Space.World);
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, Time.deltaTime * _inertiaDamping);
        }

        // Freeze z
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }
}
