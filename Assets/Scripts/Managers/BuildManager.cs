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
public class BuildManager : MonoBehaviour, IPhaseListener
{
    public static BuildManager Instance { get; private set; }

    // PROPERTIES
    [SerializeField] InputAction _input;
    [SerializeField] Transform _parentObject;
    [SerializeField] PreviewColor _previewColor;

    // REFERENCES
    GameObject _selectedPrefab;
    GameObject _previewPrefab;

    // VARIABLES
    bool _canBuild = true;
    public bool CanBuild => _canBuild && _selectedPrefab != null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple BuildManager instances found, destroying the new one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    #region PHASE LISTENER
    void OnEnable()
    {
        PhaseService.Register(this);
        OnPhaseChanged(PhaseService.Current);
    }

    void OnDisable()
    {
        PhaseService.Unregister(this);
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        _canBuild = phase == GamePhase.Day;

        if (!_canBuild)
        {
            _input.Disable();
            _selectedPrefab = null;
            if (_previewPrefab) Destroy(_previewPrefab);
        }

        Debug.LogWarning($"BuildManager phase changed: {phase}, enabled: {enabled}");
    }
    #endregion

    void Update()
    {
        if (!CanBuild) return;

        // Prefab location; follow mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = TileManager.Instance.Map.WorldToCell(mousePos);
        Vector3 worldPos = TileManager.Instance.Map.GetCellCenterWorld(tilePos);

        // Offset handling
        Vector2 objectSize = _selectedPrefab.GetComponentInChildren<SpriteRenderer>().bounds.size;
        Vector2 cellSize = TileManager.Instance.Map.cellSize;
        int tilesX = Mathf.CeilToInt(objectSize.x / cellSize.x);
        int tilesY = Mathf.CeilToInt(objectSize.y / cellSize.y);
        worldPos += new Vector3(Mathf.FloorToInt(tilesX / 2f), Mathf.FloorToInt(tilesY / 2f), 0);

        // Slightly offset by 0.5f if tilesize is evenxeven to center.
        if (tilesX % 2 == 0 || tilesY % 2 == 0)
            worldPos -= new Vector3(0.5f, 0.5f, 0);

        // Checkers
        bool isValid = TileManager.Instance.IsAreaValid(tilesX, tilesY, worldPos, TileWeight.Placeable);
        bool isWithinBounds = TileManager.Instance.IsAreaWithinBounds(tilesX, tilesY, worldPos);

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
            int id = TileManager.Instance.SetOccupiedArea(worldPos, tilesX, tilesY, TileWeight.Blocked);

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
