using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SidePanelController : MonoBehaviour
{
    [SerializeField] UIDocument _sidePanelDocument;
    [SerializeField] VisualTreeAsset _towerUITemplate;
    [SerializeField] TowerConfig _towerConfig;

    BuildManager _buildManager;
    CurrencyStorage _currencyStorage;
    VisualElement _panel;
    Button _collapseButton;
    ListView _towerList;
    List<DefenseEntry> _entries = new();

    void Awake()
    {
        _buildManager = FindFirstObjectByType<BuildManager>();
        _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
    }

    void Start()
    {
        var root = _sidePanelDocument.rootVisualElement;
        _panel = root.Q<VisualElement>("Panel");
        _collapseButton = root.Q<Button>("CollapseButton");
        _towerList = root.Q<ListView>("TowerList");

        _collapseButton.RegisterCallback<ClickEvent>(TogglePanel);
        _panel.style.display = DisplayStyle.None;
        bool isVisible = _panel.style.display == DisplayStyle.None;

        SetupEntries();
        SetupListView();
        SetupCollapseButtonPosition(isVisible);
    }

    void TogglePanel(ClickEvent evt)
    {
        bool isVisible = _panel.style.display == DisplayStyle.None;
        _panel.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        SetupCollapseButtonPosition(!isVisible);
    }

    void SetupCollapseButtonPosition(bool isCollapsed)
    {
        _collapseButton.style.top = new StyleLength(new Length(2, LengthUnit.Percent));
        _collapseButton.style.left = new StyleLength(new Length(isCollapsed ? 2 : 24, LengthUnit.Percent));
    }

    void SetupEntries()
    {
        foreach (var d in _towerConfig.DefenseEntry)
        {
            d.UpgradeLogic = () =>
            {
                if (_currencyStorage.Spend(CurrencyType.Sunlight, d.Cost))
                    _buildManager.SelectPrefab(d.DefensePrefab);
                else
                    Debug.LogWarning($"Not enough sunlight for {d.DefensePrefab.name}");
            };
            _entries.Add(d);
        }
    }

    void SetupListView()
    {
        _towerList.makeItem = () => _towerUITemplate.Instantiate();
        _towerList.bindItem = (element, index) =>
        {
            var data = _entries[index];
            var config = data.DefensePrefab.GetComponent<TowerDefense>().StatConfig;
            if (config == null) return;

            element.Q<Label>("DefenseName").text = data.DefenseEntryName;
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
