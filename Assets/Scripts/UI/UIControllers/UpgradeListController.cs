using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeListController
{
    private ListView _upgradeList;
    private VisualTreeAsset _upgradeUITemplate;
    private List<UpgradeEntry> _upgradeEntries;

    public UpgradeListController(ListView upgradeList, VisualTreeAsset upgradeUITemplate, List<UpgradeEntry> upgradeEntries)
    {
        _upgradeList = upgradeList;
        _upgradeUITemplate = upgradeUITemplate;
        _upgradeEntries = upgradeEntries;
    }

    public void Setup()
    {
        SetupUpgradeEntries();
        SetupUpgradeListView();
    }

    void SetupUpgradeEntries()
    {
        foreach (var upgrade in _upgradeEntries)
        {
            upgrade.UpgradeLogic = () =>
            {
                if (upgrade.Level >= upgrade.MaxLevel)
                {
                    Debugger.Log(DebugCategory.UI,"Upgrade maxed out!");
                    return;
                }

                float cost = upgrade.GetCost();
                if (CurrencyStorage.Instance.Spend(CurrencyType.Sunlight, cost))
                {
                    upgrade.Upgrade();

                    // Apply upgrade to all upgradable targets
                    foreach (var target in Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
                    {
                        if (target is IUpgradableCurrency upgradable)
                        {
                            upgradable.ApplyUpgrade(upgrade, upgrade.TargetCurrency);
                        }
                    }

                    _upgradeList.Rebuild(); // Refresh the list UI
                }
                else
                {
                    Debugger.LogWarning(DebugCategory.UI,"Not enough sunlight to upgrade.");
                }
            };
        }
    }

    void SetupUpgradeListView()
    {
        _upgradeList.makeItem = () => _upgradeUITemplate.Instantiate();

        _upgradeList.bindItem = (element, index) =>
        {
            var upgrade = _upgradeEntries[index];

            element.Q<Label>("UpgradeName").text = upgrade.Name;
            element.Q<Label>("Level").text = $"Lv {upgrade.Level}/{upgrade.MaxLevel}";
            element.Q<Label>("Value").text = $"Value: {upgrade.GetUpgradeValue()}";
            var button = element.Q<Button>("UpgradeButton");
            button.text = $"Cost {upgrade.GetCost()}";

            button.clickable.clicked -= upgrade.UpgradeLogic;
            button.clickable.clicked += upgrade.UpgradeLogic;
        };

        _upgradeList.itemsSource = _upgradeEntries;
        _upgradeList.selectionType = SelectionType.None;
        _upgradeList.fixedItemHeight = 80;
    }
}
