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
    BuildingEntry _entry;

    // VARIABLES
    bool _canBuild = true;
    public bool CanBuild => _canBuild && _selectedPrefab != null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debugger.LogWarning(DebugCategory.Singletons, "Multiple BuildManager instances found, destroying the new one.");
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

        Debugger.LogWarning(DebugCategory.GamePhase, $"BuildManager phase changed: {phase}, build enabled: {_canBuild}");
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
        var buildable = _selectedPrefab.GetComponent<IBuildable>();
        Vector2Int footprint = buildable.GetFootprint();

        worldPos += new Vector3(Mathf.FloorToInt(footprint.x / 2f), Mathf.FloorToInt(footprint.y / 2f), 0);

        // Slightly offset by 0.5f if tilesize is evenxeven to center.
        if (footprint.x % 2 == 0 || footprint.y % 2 == 0)
            worldPos -= new Vector3(0.5f, 0.5f, 0);

        // Checkers
        bool isValid = TileManager.Instance.IsAreaValid(footprint.x, footprint.y, worldPos, TileWeight.Placeable);
        bool isWithinBounds = TileManager.Instance.IsAreaWithinBounds(footprint.x, footprint.y, worldPos);

        if (_previewPrefab == null)
        {
            _previewPrefab = Instantiate(_selectedPrefab, transform);
            // ensure preview object is prepared immediately
            SetPreviewMode(_previewPrefab, _previewColor.NotPlaceable);
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
            GameEventsManager.RaiseBuildMode(_entry, false);

            // Update tile value
            int id = TileManager.Instance.SetOccupiedArea(worldPos, footprint.x, footprint.y, TileWeight.Blocked);

            // Instantiate
            GameObject obj = Instantiate(_selectedPrefab, worldPos, Quaternion.identity, _parentObject);
            obj.AddComponent<PlacedObject>().Init(_entry, id);
            _selectedPrefab = null;
            _input.Disable();
            Destroy(_previewPrefab);
        }
    }

    // Function called on GUI
    public void SelectPrefab(BuildingEntry entry)
    {
        GameEventsManager.RaiseBuildMode(_entry, true);
        _entry = entry;
        _selectedPrefab = entry.Prefab;
        _input.Enable();
    }

    void SetPreviewMode(GameObject gameObject, Color color)
    {
        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.color = color;
            spriteRenderer.sortingLayerName = "UIWorld";
        }

        foreach (var collider in gameObject.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }
    }
}
