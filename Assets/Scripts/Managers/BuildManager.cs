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

        // Offset handling (same logic)
        Vector2 objectSize = _selectedPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size;
        Vector2 cellSize = _tileManager.Map.cellSize;
        int tilesX = Mathf.CeilToInt(objectSize.x / cellSize.x);
        int tilesY = Mathf.CeilToInt(objectSize.y / cellSize.y);

        // Checkers
        bool isValid = IsValidTile(tilesX, tilesY, worldPos);
        bool isWithinBounds = WithinBounds(tilesX, tilesY, worldPos);

        // Centering logic, by default centre if odd x odd grid
        if (tilesX % 2 == 0 || tilesY % 2 == 0)
            worldPos -= new Vector3(0.5f, 0.5f, 0);


        // Showing the preview prefab ghost
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
            // Instantiate game object
            Instantiate(_selectedPrefab, worldPos, Quaternion.identity, _parentObject);
            _selectedPrefab = null; // Disable so update is stopped
            _input.Disable(); // Disable Input
            SetPreviewMode(_previewPrefab, false); // Undo preview mode
            Destroy(_previewPrefab); // Destroy preview object

            // Update tile value
            UpdateTile(tilesX, tilesY, worldPos);
        }
    }

    void TraverseTiles(int width, int height, Vector3 worldPos, System.Action<int, int> actionPerTile)
    {
        // Offset tile to (0, 0) to the bottom left.
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        Vector2Int originTile = _tileManager.WorldToGridIndex(worldPos) - new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int gridX = originTile.x + x;
                int gridY = originTile.y + y;

                if (!_tileManager.IsInBounds(gridX, gridY))
                    continue;

                actionPerTile?.Invoke(gridX, gridY);
            }
        }
    }

    bool IsValidTile(int width, int height, Vector3 worldPos)
    {
        bool allWalkable = true;

        TraverseTiles(width, height, worldPos, (x, y) =>
        {
            if (_tileManager.Grid[x, y] != TileWeight.Walkable)
                allWalkable = false;
        });

        return allWalkable;
    }

    bool WithinBounds(int width, int height, Vector3 worldPos)
    {
        int offsetX = Mathf.FloorToInt(width / 2f);
        int offsetY = Mathf.FloorToInt(height / 2f);
        Vector2Int originTile = _tileManager.WorldToGridIndex(worldPos) - new Vector2Int(offsetX, offsetY);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int gridX = originTile.x + x;
                int gridY = originTile.y + y;

                if (!_tileManager.IsInBounds(gridX, gridY))
                    return false;
            }
        }

        return true;
    }

    void UpdateTile(int width, int height, Vector3 worldPos)
    {
        TraverseTiles(width, height, worldPos, (x, y) =>
        {
            _tileManager.SetOccupied(new Vector2Int(x, y), TileWeight.Blocked);
        });
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
