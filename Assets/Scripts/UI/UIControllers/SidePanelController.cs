using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SidePanelController : MonoBehaviour
{
    public static SidePanelController Instance { get; private set; }

    [SerializeField] UIDocument _sidePanelDocument;
    [SerializeField] VisualTreeAsset _towerUITemplate;
    [SerializeField] TowerConfig _towerConfig;

    VisualElement _panel;
    Button _collapseButton;
    ListView _towerList;
    List<BuildingEntry> _entries = new();
    bool _isVisible = false;


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
        _panel = root.Q<VisualElement>("Panel");
        _collapseButton = root.Q<Button>("CollapseButton");
        _towerList = root.Q<ListView>("TowerList");

        _collapseButton.RegisterCallback<ClickEvent>(CollapseButtonClicked);

        _panel.AddToClassList("hidden");
        _isVisible = false;

        SetupEntries();
        SetupListView();
        SetupCollapseButtonPosition(true); // Initially collapsed

    }

    void CollapseButtonClicked(ClickEvent evt)
    {
        _isVisible = !_isVisible;
        TogglePanel(_isVisible);
    }

    void BuildModeChanged(BuildingEntry entry, bool isBuilding)
    {
        TogglePanel(!isBuilding);
    }

    public void TogglePanel(bool isVisible)
    {
        if (isVisible)
        {
            _panel.RemoveFromClassList("hidden");
            _panel.AddToClassList("visible");
            Debug.Log("Added 'visible' class, removed 'hidden'");
        }
        else
        {
            _panel.RemoveFromClassList("visible");
            _panel.AddToClassList("hidden");
            Debug.Log("Added 'hidden' class, removed 'visible'");

        }

        SetupCollapseButtonPosition(!isVisible);
    }

    void SetupCollapseButtonPosition(bool isCollapsed)
    {
        _collapseButton.style.top = new StyleLength(new Length(2, LengthUnit.Percent));
        _collapseButton.style.left = new StyleLength(new Length(isCollapsed ? 2 : 25, LengthUnit.Percent));
    }

    void SetupEntries()
    {
        foreach (var d in _towerConfig.DefenseEntry)
        {
            d.UpgradeLogic = () =>
            {
                if (CurrencyStorage.Instance.Spend(CurrencyType.Sunlight, d.Cost))
                {
                    BuildManager.Instance.SelectPrefab(d);
                }
                else
                    Debug.LogWarning($"Not enough sunlight for {d.Prefab.name}");
            };
            _entries.Add(d);
        }
    }

    // TODO: Dowee, using strings is very prone error. we'll have to use Unity Localization for this.
    void SetupListView()
    {
        _towerList.makeItem = () => _towerUITemplate.Instantiate();
        _towerList.bindItem = (element, index) =>
        {
            var data = _entries[index];
            var config = data.Prefab.GetComponent<TowerDefense>().StatConfig;
            if (config == null) return;

            element.Q<Label>("DefenseName").text = data.BuildEntryName;
            element.Q<Image>("Thumbnail").sprite = data.Thumbnail;
            element.Q<Label>("DMG").text = "DMG: " + config.GetValue(TowerStats.Attack);
            element.Q<Label>("RNG").text = "RNG: " + config.GetValue(TowerStats.Range);
            element.Q<Label>("SPD").text = "SPD: " + config.GetValue(TowerStats.Speed);
            element.Q<Button>("BuildButton").text = $"Cost {data.Cost}";
            element.Q<Button>("BuildButton").clickable.clicked += data.UpgradeLogic;
        };
        _towerList.itemsSource = _entries;
        _towerList.selectionType = SelectionType.None;
        _towerList.fixedItemHeight = 100;
    }
}
