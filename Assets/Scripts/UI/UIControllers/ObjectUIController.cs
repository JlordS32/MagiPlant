using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectUIController : MonoBehaviour
{
    public static ObjectUIController Instance { get; private set; }

    [SerializeField] UIDocument _uiDocument;
    VisualElement _panel;
    Label _objectNameLabel;
    Button _upgradeBtn;
    Button _infoBtn;
    PlacedObject _currentPlacedObject;

    bool _enableMouseClickCheck = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debugger.LogWarning(DebugCategory.Singletons, "Multiple instances of ObjectUIController detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _panel = _uiDocument.rootVisualElement.Q<VisualElement>("Panel");
        if (_panel == null)
        {
            Debugger.LogError(DebugCategory.UI, "Panel element not found in the UIDocument.");
            return;
        }

        _panel.AddToClassList("hidden");
        _panel.RemoveFromClassList("active");
    }

    void Start()
    {
        var root = _uiDocument.rootVisualElement;
        _objectNameLabel = root.Q<Label>("ObjectName");
        _upgradeBtn = root.Q<Button>("UpgradeBtn");
        _infoBtn = root.Q<Button>("InfoBtn");

        // Registering callbacks
        _upgradeBtn.RegisterCallback<ClickEvent>(_ =>
        {
            if (_currentPlacedObject != null)
            {
                ToggleObjectUI(false);
                ResourceData data = _currentPlacedObject.GetComponent<ResourceBuilding>().Data;

                if (data != null)
                {
                    data.UpgradeAll();
                }
            }
        });
        
        _infoBtn.RegisterCallback<ClickEvent>(_ =>
        {
            if (_currentPlacedObject != null)
            {
                ToggleObjectUI(false);
                ObjectDialogController.Instance.Show(_currentPlacedObject);
            }
        });
    }

    public void ToggleObjectUI(bool enabled)
    {
        _enableMouseClickCheck = enabled;
    }

    void Update()
    {
        if (!_enableMouseClickCheck) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            if (IsPointerOverUI(mousePos))
                return;

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

    bool IsPointerOverUI(Vector2 screenPosition)
    {
        var panel = _uiDocument.rootVisualElement?.panel;
        if (panel == null) return false;

        // Flip Y because UI Toolkit uses bottom-left origin
        screenPosition.y = Screen.height - screenPosition.y;
        var picked = panel.Pick(screenPosition);

        // If any element besides the root is picked, it's a UI element
        return picked != null && picked != _uiDocument.rootVisualElement;
    }


    bool ClickedOnPlacedObject()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPoint);

        if (hit != null)
        {
            // Check if the hit object is a PlacedObject
            if (hit.TryGetComponent<PlacedObject>(out var placedObject))
            {
                _currentPlacedObject = placedObject;
                Show(placedObject.Entry);
                return true;
            }

            // If it's not a PlacedObject, we hide the UI
            return false;
        }

        // Nothing hit at all
        return false;
    }

}
