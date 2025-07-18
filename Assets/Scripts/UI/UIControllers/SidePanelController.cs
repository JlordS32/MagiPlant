using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SidePanelController : MonoBehaviour
{
    public static SidePanelController Instance { get; private set; }

    [Header("UI Assets")]
    [SerializeField] UIDocument _sidePanelDocument;
    [SerializeField] VisualTreeAsset _towerUITemplate;
    [SerializeField] VisualTreeAsset _upgradeUITemplate;

    [Header("Config Data")]
    [SerializeField] BuildingConfigEntry _buildingConfigEntry;
    [SerializeField] List<UpgradeEntry> _upgradeEntries = new();

    // UI Elements
    VisualElement _panel;
    Button _collapseButton;
    ListView _towerList;
    ListView _upgradeList;
    Button _towerTab;
    Button _upgradeTab;

    bool _isVisible = false;

    TowerListController _towerController;
    UpgradeListController _upgradeController;

    void OnEnable() => GameEventsManager.OnBuildMode += BuildModeChanged;
    void OnDisable() => GameEventsManager.OnBuildMode -= BuildModeChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of SidePanelController detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        var root = _sidePanelDocument.rootVisualElement;

        // Initialize UI elements
        _panel = root.Q<VisualElement>("Panel");
        _collapseButton = root.Q<Button>("CollapseButton");
        _towerList = root.Q<ListView>("TowerList");
        _upgradeList = root.Q<ListView>("UpgradeList");
        _towerTab = root.Q<Button>("TowerTab");
        _upgradeTab = root.Q<Button>("UpgradeTab");

        // Register callbacks to the buttons
        _collapseButton.RegisterCallback<ClickEvent>(CollapseButtonClicked);
        _towerTab.RegisterCallback<ClickEvent>(OnTowerButtonClicked);
        _upgradeTab.RegisterCallback<ClickEvent>(OnUpgradeButtonClicked);

        _panel.AddToClassList("hidden");

        _isVisible = false;

        // Initialize sub-controllers with dependencies
        _towerController = new TowerListController(_towerList, _towerUITemplate, _buildingConfigEntry);
        _upgradeController = new UpgradeListController(_upgradeList, _upgradeUITemplate, _upgradeEntries);

        // Setup UI lists
        _towerController.Setup();
        _upgradeController.Setup();
        SetupCollapseButtonPosition(true); // Initially collapsed
    }

    void CollapseButtonClicked(ClickEvent evt)
    {
        _isVisible = !_isVisible;
        TogglePanel(_isVisible);
    }

    void OnTowerButtonClicked(ClickEvent evt)
    {
        _towerTab.AddToClassList("active");
        _upgradeTab.RemoveFromClassList("active");
        _towerList.AddToClassList("active");
        _upgradeList.RemoveFromClassList("active");
    }

    void OnUpgradeButtonClicked(ClickEvent evt)
    {
        _towerTab.RemoveFromClassList("active");
        _upgradeTab.AddToClassList("active");
        _upgradeList.AddToClassList("active");
        _towerList.RemoveFromClassList("active");
    }

    void BuildModeChanged(BuildingEntry entry, bool isBuilding)
    {
        TogglePanel(!isBuilding);
    }

    public void TogglePanel(bool isVisible)
    {
        _panel.EnableInClassList("visible", isVisible);
        _panel.EnableInClassList("hidden", !isVisible);
        SetupCollapseButtonPosition(!isVisible);
    }

    void SetupCollapseButtonPosition(bool isCollapsed)
    {
        _collapseButton.style.top = new StyleLength(new Length(2, LengthUnit.Percent));
        _collapseButton.style.left = new StyleLength(new Length(isCollapsed ? 2 : 25, LengthUnit.Percent));
    }
}
