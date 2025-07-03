using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [SerializeField] InputAction _input;
    [SerializeField] GameObject _objects;
    [SerializeField] Transform _parentObject;

    // VARIABLES
    TileManager _tileManager;
    Vector3 _worldPos;

    void Awake()
    {
        _tileManager = FindFirstObjectByType<TileManager>();
        _input.Enable();
    }

    void OnDestroy()
    {
        _input.Disable();
    }

    void Update()
    {
        if (_input.WasReleasedThisFrame())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePos = _tileManager.Map.WorldToCell(mousePos);
            _worldPos = _tileManager.Map.GetCellCenterWorld(tilePos);
            Build();
        }
    }

    // TODO: Update grid on object placement
    // TODO: Prevent placing objects if tile is not 1
    public void Build()
    {
        Instantiate(_objects, _worldPos, Quaternion.identity, _parentObject);
    }

    public void Build(GameObject gameObject, Vector3 worldPos)
    {
        Instantiate(gameObject, worldPos, Quaternion.identity, _parentObject);
    }

    // TODO: Dynamically adjust the wirecube depending on the amount of tiles the gameobject takes up.
    // TODO: Extract this logic so it works on game mode.
    void OnDrawGizmos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = _tileManager.Map.WorldToCell(mousePos);
        Vector3 adjustedPos = _tileManager.Map.GetCellCenterWorld(tilePos);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(adjustedPos, _tileManager.Map.cellSize);
    }
}
