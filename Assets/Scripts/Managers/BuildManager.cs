using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] InputAction _input;
    [SerializeField] Transform _parentObject;

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

        // Checkers
        bool isValid = _tileManager.IsAreaValid(tilesX, tilesY, worldPos);
        bool isWithinBounds = _tileManager.IsAreaWithinBounds(tilesX, tilesY, worldPos);

        // Slightly offset by 0.5f if tilesize is evenxeven. This is because there's no center.
        if (tilesX % 2 == 0 || tilesY % 2 == 0)
            worldPos -= new Vector3(0.5f, 0.5f, 0);

        if (_previewPrefab == null)
        {
            _previewPrefab = Instantiate(_selectedPrefab);
            SetPreviewMode(_previewPrefab, true);
        }
        _previewPrefab.transform.position = worldPos;
        SetPreviewMode(_previewPrefab, true, isValid, isWithinBounds);


        // WARNING: No bound here, might need in the future.
        if (_input.WasReleasedThisFrame() && isValid && isWithinBounds)
        {
            // Update tile value
            int id = _tileManager.SetOccupiedArea(worldPos, tilesX, tilesY, TileWeight.Blocked);

            // Instantiate
            GameObject obj = Instantiate(_selectedPrefab, worldPos, Quaternion.identity, _parentObject);
            obj.GetComponent<PlacedObject>().TileId = id;
            _selectedPrefab = null; // Disable so update is stopped
            _input.Disable(); // Disable Input
            SetPreviewMode(_previewPrefab, false); // Undo preview mode
            Destroy(_previewPrefab); // Destroy preview object
        }
    }

    // Function called on GUI
    public void SelectPrefab(GameObject prefab)
    {
        _selectedPrefab = prefab;
        _input.Enable();
    }

    void SetPreviewMode(GameObject gameObject, bool isPreview, bool isValid = true, bool isWithinBounds = true)
    {
        Color color = isValid && isWithinBounds ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0f, 0f, 0.5f); // red if invalid

        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = color;
        }

        foreach (var collider in gameObject.GetComponents<Collider2D>())
        {
            collider.enabled = !isPreview;
        }
    }
}
