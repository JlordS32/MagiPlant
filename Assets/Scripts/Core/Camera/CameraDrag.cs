using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraDrag : MonoBehaviour
{
    [SerializeField] float _dragSpeed = 2f;
    [SerializeField] float _inertiaDuration = 0.5f;
    [SerializeField] float _inertiaDamping = 5f;
    [SerializeField] Tilemap _grid;

    Vector3 _dragOrigin;
    Vector3 _velocity;
    bool _isDragging = false;
    float _inertiaTime = 0;
    Bounds _worldBounds;
    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void OnDrawGizmos()
    {
        if (_grid == null) return;

        _grid.CompressBounds();
        Bounds local = _grid.localBounds;
        Vector3 worldCenter = _grid.transform.TransformPoint(local.center);
        Vector3 worldSize = Vector3.Scale(local.size, _grid.transform.lossyScale);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(worldCenter, worldSize);
    }

    void Update()
    {
        if (_grid == null || _cam == null) return;

        UpdateTileMapBounds();

        if (Input.GetMouseButtonDown(0))
        {
            _dragOrigin = Input.mousePosition;
            _isDragging = true;
            _velocity = Vector3.zero;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = _cam.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
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

        ClampCamera();
    }

    void UpdateTileMapBounds()
    {
        _grid.CompressBounds();
        _worldBounds = _grid.localBounds;
        _worldBounds.center = _grid.transform.TransformPoint(_worldBounds.center);
        _worldBounds.size = Vector3.Scale(_worldBounds.size, _grid.transform.lossyScale);
    }

    void ClampCamera()
    {
        float camHeight = _cam.orthographicSize;
        float camWidth = camHeight * _cam.aspect;

        Vector3 pos = transform.position;

        float minX = _worldBounds.min.x + camWidth;
        float maxX = _worldBounds.max.x - camWidth;
        float minY = _worldBounds.min.y + camHeight;
        float maxY = _worldBounds.max.y - camHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = 0;

        transform.position = pos;
    }
}
