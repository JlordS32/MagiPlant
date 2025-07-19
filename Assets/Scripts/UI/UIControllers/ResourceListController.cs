using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ResourceListController
{
    private ListView _resourceList;
    private VisualTreeAsset _resourceUITemplate;
    private BuildingConfigEntry _buildingConfigEntry;
    private List<BuildingEntry> _entries = new();

    public ResourceListController(ListView resourceList, VisualTreeAsset resourceUITemplate, BuildingConfigEntry buildingConfigEntry)
    {
        _resourceList = resourceList;
        _resourceUITemplate = resourceUITemplate;
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
            if (d.Category != BuildingType.Resources) continue;

            d.BuildingLogic = () =>
            {
                if (CurrencyStorage.Instance.Spend(CurrencyType.Sunlight, d.Cost))
                {
                    BuildManager.Instance.SelectPrefab(d);
                }
                else
                    Debugger.LogWarning(DebugCategory.UI, $"Not enough sunlight for {d.Prefab.name}");
            };
            _entries.Add(d);
        }
    }

    void SetupListView()
    {
        _resourceList.makeItem = () => _resourceUITemplate.Instantiate();
        _resourceList.bindItem = (element, index) =>
        {
            var data = _entries[index];
            var generator = data.Prefab.GetComponent<ResourceGenerator>();
            if (generator == null) return;

            element.Q<Label>("ResourceName").text = data.BuildEntryName;
            element.Q<Image>("Thumbnail").sprite = data.Thumbnail;
            element.Q<Label>("GEN").text = $"+{generator.GenerateAmount}/{generator.Interval}s";
            element.Q<Label>("TYPE").text = generator.ResourceType.ToString();
            var buildButton = element.Q<Button>("BuildButton");
            buildButton.text = $"Cost {data.Cost}";

            buildButton.clickable.clicked -= data.BuildingLogic;
            buildButton.clickable.clicked += data.BuildingLogic;
        };

        _resourceList.itemsSource = _entries;
        _resourceList.selectionType = SelectionType.None;
        _resourceList.fixedItemHeight = 100;
    }
}
