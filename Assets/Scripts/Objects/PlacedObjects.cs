using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public int TileId;
    ObjectUIController _objectUIController;

    void Awake()
    {
        _objectUIController = FindFirstObjectByType<ObjectUIController>();
    }

    void OnMouseUpAsButton()
    {
        _objectUIController.Show();
    }
}
