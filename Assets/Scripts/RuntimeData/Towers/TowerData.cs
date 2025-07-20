using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TowerData
{
    Dictionary<TowerStats, float> _stats = new();
    TowerStatConfig _config;

    int _level = 1;
    public int Level => _level;

    // Constructor
    public TowerData(TowerStatConfig config)
    {
        _config = config;

        // Initialising all stats to 0
        foreach (TowerStats stat in Enum.GetValues(typeof(TowerStats)))
        {
            _stats[stat] = 0f;
        }

        // Set initial values based on config
        Reset();

        foreach (var stat in _stats)
        {
            Debugger.Log(DebugCategory.Towers, $"Level {_level}: {stat.Key}: {stat.Value}");
        }
    }

    public float Get(TowerStats stat) => _stats[stat];

    public void Set(TowerStats stat, float value)
    {
        _stats[stat] = value;
        GameEventsManager.RaiseTowerStatUpdate(stat, value);
    }

    public void Upgrade(TowerStats stat)
    {
        if (!_stats.ContainsKey(stat))
        {
            Debugger.LogWarning(DebugCategory.Towers, $"Tower stat '{stat}' not found.");
            return;
        }

        float newValue = _config.GetValue(stat, _level);

        _stats[stat] = newValue;

        GameEventsManager.RaiseTowerStatUpdate(stat, newValue);
    }

    public void UpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            Debugger.LogWarning(DebugCategory.Towers, "Attempting to upgrade tower beyond max level.");
            return;
        }

        float cost = _config.GetCost(_level);
        Debugger.Log(DebugCategory.Towers, $"Current cost: {cost}");
        var currencies = _config.CostType;

        // Check if player can afford upgrade
        bool canAfford = currencies.All(currency => CurrencyStorage.Instance.CanAfford(currency, cost));
        if (!canAfford)
        {
            Debugger.LogWarning(DebugCategory.Towers, "Insufficient currency to upgrade.");
            return;
        }

        // Spend the resources
        // WARNING: If bug occurs, we might have to add a rollback feature just in case.
        foreach (var currency in currencies)
        {
            CurrencyStorage.Instance.Spend(currency, cost);
        }

        // Upgrade levle
        _level++;

        foreach (var entry in _config.Stats)
        {
            Upgrade(entry.Stat);
            Debugger.Log(DebugCategory.Towers, $"Level {_level}: {entry.Stat}: {_stats[entry.Stat]}");
        }
    }

    public void Reset()
    {
        foreach (TowerStats stat in Enum.GetValues(typeof(TowerStats)))
        {
            _stats[stat] = _config.GetValue(stat, 0);
        }

        _level = 1;
        GameEventsManager.RaiseTowerStatReset();
    }
}
