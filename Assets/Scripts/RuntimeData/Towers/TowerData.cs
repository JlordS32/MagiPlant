using System;
using System.Linq;

[Serializable]
public class TowerData : Data<TowerStats, TowerStatConfig>
{
    public TowerData(TowerStatConfig config)
    {
        Init(config, DebugCategory.Towers);
    }
    
    public void Upgrade(TowerStats stat)
    {
        if (!_data.ContainsKey(stat))
        {
            Debugger.LogWarning(_debugCategory, $"Tower stat '{stat}' not found.");
            return;
        }

        float newValue = _config.GetValue(stat, _level);
        _data[stat] = newValue;
        RaiseStatUpdateEvent(stat, newValue);
    }

    public void UpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            Debugger.LogWarning(_debugCategory, "Attempting to upgrade tower beyond max level.");
            return;
        }

        float cost = _config.GetCost(_level);
        var currencies = _config.CostType;

        bool canAfford = currencies.All(currency => CurrencyStorage.Instance.CanAfford(currency, cost));
        if (!canAfford)
        {
            Debugger.LogWarning(_debugCategory, "Insufficient currency to upgrade.");
            return;
        }

        foreach (var currency in currencies)
            CurrencyStorage.Instance.Spend(currency, cost);

        _level++;

        foreach (var entry in _config.Stats)
        {
            Upgrade(entry.Stat);
            Debugger.Log(_debugCategory, $"Level {_level}: {entry.Stat}: {_data[entry.Stat]}");
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
