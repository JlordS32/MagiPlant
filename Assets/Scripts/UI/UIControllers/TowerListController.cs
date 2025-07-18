using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerListController
{
    private ListView _towerList;
    private VisualTreeAsset _towerUITemplate;
    private BuildingConfigEntry _buildingConfigEntry;
    private List<BuildingEntry> _entries = new();

    public TowerListController(ListView towerList, VisualTreeAsset towerUITemplate, BuildingConfigEntry buildingConfigEntry)
    {
        _towerList = towerList;
        _towerUITemplate = towerUITemplate;
        _buildingConfigEntry = buildingConfigEntry;
    }

    public void Setup()
    {
        SetupEntries();
        SetupListView();
    }

    void SetupEntries()
    {
        foreach (var d in _buildingConfigEntry.BuildingEntries)
        {
            d.BuildingLogic = () =>
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
            var buildButton = element.Q<Button>("BuildButton");
            buildButton.text = $"Cost {data.Cost}";

            buildButton.clickable.clicked -= data.BuildingLogic;
            buildButton.clickable.clicked += data.BuildingLogic;
        };
        _towerList.itemsSource = _entries;
        _towerList.selectionType = SelectionType.None;
        _towerList.fixedItemHeight = 100;
    }
}
