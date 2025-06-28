using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float _zoomValue = 0.1f;
    [SerializeField] float _minZoom = 1f;
    [SerializeField] float _maxZoom = 20f;
    [SerializeField] float _zoomLerpSpeed = 10f;
    [SerializeField] Tilemap _grid;

    Camera _cam;
    float _targetZoom;

    void Start()
    {
        _cam = Camera.main;
        _targetZoom = _cam.orthographicSize;
    }

    void Update()
    {
        if (_grid == null || _cam == null) return;

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            Bounds bounds = GetWorldBounds();
            float maxZoomFromBounds = GetMaxZoomFit(bounds);

            _targetZoom -= mouseScroll * _zoomValue * 10f;
            _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, Mathf.Min(_maxZoom, maxZoomFromBounds));
        }

        _cam.orthographicSize = Mathf.Lerp(_cam.orthographicSize, _targetZoom, Time.deltaTime * _zoomLerpSpeed);
    }

    Bounds GetWorldBounds()
    {
        _grid.CompressBounds();
        Bounds local = _grid.localBounds;
        Vector3 worldCenter = _grid.transform.TransformPoint(local.center);
        Vector3 worldSize = Vector3.Scale(local.size, _grid.transform.lossyScale);
        return new Bounds(worldCenter, worldSize);
    }

    float GetMaxZoomFit(Bounds bounds)
    {
        float maxY = bounds.size.y / 2f;
        float maxX = (bounds.size.x / 2f) / _cam.aspect;
        return Mathf.Min(maxX, maxY);
    }
}
