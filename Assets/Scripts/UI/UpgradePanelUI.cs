using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] GameObject _upgradeComponent;

    Dictionary<(UpgradeType, CurrencyType), UpgradeUI> _uiMap = new();

    public void Build(List<UpgradeEntry> entries)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        _uiMap.Clear();

        foreach (var entry in entries)
        {
            GameObject go = Instantiate(_upgradeComponent, transform);
            var ui = go.GetComponent<UpgradeUI>();

            ui.Setup(entry.Name, () => entry.UpgradeLogic.Invoke());
            _uiMap[(entry.Type, entry.TargetCurrency)] = ui;

            ui.Refresh(
                $"Rate: {entry.GetUpgradeValue():F2}",
                $"Lvl: {entry.Level}/{entry.MaxLevel}",
                $"Cost: {entry.GetCost()} suns"
            );
        }
    }

    public void RefreshUI(UpgradeType type, CurrencyType currency, float rate, int level, float cost, int maxLevel)
    {
        if (_uiMap.TryGetValue((type, currency), out var ui))
        {
            ui.Refresh(
                $"Rate: {rate:F2}",
                $"Lvl: {level}/{maxLevel}",
                $"Cost: {cost} suns"
            );
        }
    }

}
