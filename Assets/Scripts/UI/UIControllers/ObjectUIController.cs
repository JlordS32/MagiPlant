using UnityEngine;
using UnityEngine.UIElements;

public class ObjectUIController : MonoBehaviour
{
    public static ObjectUIController Instance { get; private set; }

    [SerializeField] UIDocument _uiDocument;
    VisualElement _panel;
    Label _objectNameLabel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of ObjectUIController detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _panel = _uiDocument.rootVisualElement.Q<VisualElement>("Panel");
        if (_panel == null)
        {
            Debug.LogError("Panel element not found in the UIDocument.");
            return;
        }

        _panel.AddToClassList("hidden");
        _panel.RemoveFromClassList("active");
    }

    void Start()
    {
        var root = _uiDocument.rootVisualElement;
        _objectNameLabel = root.Q<Label>("ObjectName");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!ClickedOnPlacedObject())
            {
                Hide();
            }
        }
    }

    public void Show(BuildingEntry entry)
    {
        if (_objectNameLabel != null)
            _objectNameLabel.text = entry.BuildEntryName;

        if (_panel != null && _panel.ClassListContains("hidden"))
        {
            _panel.RemoveFromClassList("hidden");
            _panel.AddToClassList("active");
        }
    }

    public void Hide()
    {
        _panel.RemoveFromClassList("active");
        _panel.AddToClassList("hidden");
    }

    bool ClickedOnPlacedObject()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);

        if (hit != null)
        {
            if (hit.TryGetComponent<PlacedObject>(out var placedObject))
            {
                Debug.Log("clicking placed object");
                Show(placedObject.Entry);
                return true;
            }

            // Clicked something else (like a tile) → treat as invalid
            Debug.Log("hit something that's not a PlacedObject");
            return false;
        }

        // Nothing hit at all
        return false;
    }

}
