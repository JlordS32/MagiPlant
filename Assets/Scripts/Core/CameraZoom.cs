using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float _zoomValue = 0.1f;
    [SerializeField] float _minZoom = 1f;
    [SerializeField] float _maxZoom = 20f;

    void Update()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            Camera.main.orthographicSize -= mouseScroll * _zoomValue * 10f;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, _minZoom, _maxZoom);
        }
    }
}
