using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class TowerData : Data<TowerStats, TowerStatConfig>
{
    public TowerData(TowerStatConfig config)
    {
        Init(config);
    }
    
    public void Upgrade(TowerStats stat)
    {
        if (!_data.ContainsKey(stat))
        {
            Debugger.LogWarning(_config.DebugCategory, $"Tower stat '{stat}' not found.");
            return;
        }

        float newValue = Mathf.FloorToInt(_config.GetValue(stat, _level));
        _data[stat] = newValue;
        RaiseStatUpdateEvent(stat, newValue);
    }

    public void UpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            Debugger.LogWarning(_config.DebugCategory, "Attempting to upgrade tower beyond max level.");
            return;
        }

        float cost = _config.GetCost(_level);
        var currencies = _config.CostType;

        bool canAfford = currencies.All(currency => CurrencyStorage.Instance.CanAfford(currency, cost));
        if (!canAfford)
        {
            Debugger.LogWarning(_config.DebugCategory, "Insufficient currency to upgrade.");
            return;
        }

        foreach (var currency in currencies)
            CurrencyStorage.Instance.Spend(currency, cost);

        _level++;
        RaiseLevelChanged(_level);

        foreach (var entry in _config.Stats)
        {
            Upgrade(entry.Stat);
            Debugger.Log(_config.DebugCategory, $"Level {_level}: {entry.Stat}: {_data[entry.Stat]}");
        }
    }

    protected override void RaiseStatUpdateEvent(TowerStats stat, float value)
    {
        GameEventsManager.RaiseTowerStatUpdate(stat, value);
    }

    protected override void RaiseResetUpdateEvent()
    {
        GameEventsManager.RaiseTowerStatReset();
    }
}
