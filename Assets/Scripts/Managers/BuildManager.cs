using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct PreviewColor
{
    public Color Placeable;
    public Color NotPlaceable;
}

// WARNING: Read before making any modifications.
public class BuildManager : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] InputAction _input;
    [SerializeField] Transform _parentObject;
    [SerializeField] PreviewColor _previewColor;

    // REFERENCES
    TileManager _tileManager;
    GameObject _selectedPrefab;
    GameObject _previewPrefab;

    void Awake()
    {
        _tileManager = FindFirstObjectByType<TileManager>();
    }

    void Update()
    {
        if (_selectedPrefab == null) return;

        // Prefab location; follow mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = _tileManager.Map.WorldToCell(mousePos);
        Vector3 worldPos = _tileManager.Map.GetCellCenterWorld(tilePos);

        // Offset handling
        Vector2 objectSize = _selectedPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size;
        Vector2 cellSize = _tileManager.Map.cellSize;
        int tilesX = Mathf.CeilToInt(objectSize.x / cellSize.x);
        int tilesY = Mathf.CeilToInt(objectSize.y / cellSize.y);
        worldPos += new Vector3(Mathf.FloorToInt(tilesX / 2f), Mathf.FloorToInt(tilesY / 2f), 0);

        // Slightly offset by 0.5f if tilesize is evenxeven to center.
        if (tilesX % 2 == 0 || tilesY % 2 == 0)
            worldPos -= new Vector3(0.5f, 0.5f, 0);

        // Checkers
        bool isValid = _tileManager.IsAreaValid(tilesX, tilesY, worldPos, TileWeight.Placeable);
        bool isWithinBounds = _tileManager.IsAreaWithinBounds(tilesX, tilesY, worldPos);
        Debug.Log(isWithinBounds);

        if (_previewPrefab == null)
        {
            _previewPrefab = Instantiate(_selectedPrefab, transform);
        }
        else
        {
            if (isValid && isWithinBounds)
                SetPreviewMode(_previewPrefab, _previewColor.Placeable);
            else
                SetPreviewMode(_previewPrefab, _previewColor.NotPlaceable);
            _previewPrefab.transform.position = worldPos;
        }

        if (_input.WasReleasedThisFrame() && isValid && isWithinBounds)
        {
            // Update tile value
            int id = _tileManager.SetOccupiedArea(worldPos, tilesX, tilesY, TileWeight.Blocked);

            // Instantiate
            GameObject obj = Instantiate(_selectedPrefab, worldPos, Quaternion.identity, _parentObject);
            obj.AddComponent<PlacedObject>().TileId = id;
            _selectedPrefab = null;
            _input.Disable();
            Destroy(_previewPrefab);
        }
    }

    // Function called on GUI
    public void SelectPrefab(GameObject prefab)
    {
        _selectedPrefab = prefab;
        _input.Enable();
    }

    void SetPreviewMode(GameObject gameObject, Color color)
    {
        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = color;
            spriteRenderer.sortingLayerName = "UIWorld";
        }

        foreach (var collider in gameObject.GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }
    }
}
