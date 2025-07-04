using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [SerializeField] InputAction _input;
    [SerializeField] Transform _parentObject;

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

        // Centering logic, by default centre if odd x odd grid
        if (tilesX % 2 == 0 || tilesY % 2 == 0)
            worldPos -= new Vector3(0.5f, 0.5f, 0);

        // Showing the preview prefab ghost
        if (_previewPrefab == null)
        {
            _previewPrefab = Instantiate(_selectedPrefab);
            SetPreviewMode(_previewPrefab, true);
        }
        else
        {
            _previewPrefab.transform.position = worldPos;
        }

        // WARNING: No bound here, might need in the future.
        if (_input.WasReleasedThisFrame() && IsValidTile(tilesX, tilesY, worldPos))
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
        Vector3Int originTile = _tileManager.Map.WorldToCell(worldPos);
        int originX = originTile.x - _tileManager.Bounds.xMin;
        int originY = originTile.y - _tileManager.Bounds.yMin;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int gridX = originX + x;
                int gridY = originY + y;

                if (gridX < 0 || gridX >= _tileManager.Grid.GetLength(0) ||
                    gridY < 0 || gridY >= _tileManager.Grid.GetLength(1))
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

    void UpdateTile(int width, int height, Vector3 worldPos)
    {
        TraverseTiles(width, height, worldPos, (x, y) =>
        {
            _tileManager.UpdateGrid(x, y, TileWeight.Blocked);
        });
    }

    // Function called on GUI
    public void SelectPrefab(GameObject prefab)
    {
        _selectedPrefab = prefab;
        _input.Enable();
    }

    void SetPreviewMode(GameObject gameObject, bool isPreview)
    {
        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Color spriteColor = spriteRenderer.color;
            spriteColor.a = isPreview ? 0.5f : 1f;
            spriteRenderer.color = spriteColor;
        }

        // Disable colliders if any
        foreach (var collider in gameObject.GetComponents<Collider2D>())
        {
            collider.enabled = !isPreview;
        }
    }

    // TODO: Extract this logic so it works on game mode.
    void OnDrawGizmos()
    {
        if (_tileManager == null || _selectedPrefab == null) return;

        Vector2 objectSize = _selectedPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size;
        Vector2 cellSize = _tileManager.Map.cellSize;

        int tilesX = Mathf.CeilToInt(objectSize.x / cellSize.x);
        int tilesY = Mathf.CeilToInt(objectSize.y / cellSize.y);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = _tileManager.Map.WorldToCell(mousePos);

        // Center alignment
        tilePos -= new Vector3Int(
            Mathf.FloorToInt(tilesX / 2f),
            Mathf.FloorToInt(tilesY / 2f),
            0
        );

        Gizmos.color = Color.green;

        for (int y = 0; y < tilesY; y++)
        {
            for (int x = 0; x < tilesX; x++)
            {
                Vector3Int cell = tilePos + new Vector3Int(x, y, 0);
                Vector3 cellCenter = _tileManager.Map.GetCellCenterWorld(cell);
                Gizmos.DrawWireCube(cellCenter, (Vector3)cellSize);
            }
        }
    }
}
