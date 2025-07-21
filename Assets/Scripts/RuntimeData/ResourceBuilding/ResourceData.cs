using System;
using System.Linq;

[Serializable]
public class ResourceData : Data<CurrencyType, ResourceStatConfig>
{
    // Constructor
    public ResourceData(ResourceStatConfig config)
    {
        _config = config;

        // Initialise values
        foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
        {
            _data[currency] = 0f;
        }

        Reset();

        foreach (var entry in _data)
        {
            Debugger.Log(DebugCategory.Resources, $"Level {_level}: {entry.Key}: {entry.Value}");
        }
    }

    #region GETTERS & SETTERS
    #endregion
    #region UPGRADE
    public void Upgrade(CurrencyType currency)
    {
        float newValue = _config.GetValue(currency, _level);
        _data[currency] = newValue;
        RaiseStatUpdateEvent(currency, newValue);
    }

    public void UpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            Debugger.LogWarning(DebugCategory.Resources, "Attempting to upgrade resources beyond max level.");
            return;
        }

        float cost = _config.GetCost(_level);
        Debugger.Log(DebugCategory.Resources, $"Current Cost: {cost}");
        var currencies = _config.CostType;

        // Check if player can afford upgrade
        bool canAfford = currencies.All(currency => CurrencyStorage.Instance.CanAfford(currency, cost));
        if (!canAfford)
        {
            Debugger.LogWarning(DebugCategory.Resources, "Insufficient currency to upgrade.");
            return;
        }

        // Spend the resources
        // WARNING: If bug occurs, we might have to add a rollback feature just in case.
        foreach (var currency in currencies)
        {
            CurrencyStorage.Instance.Spend(currency, cost);
        }

        _level++;

        foreach (var entry in _config.Resources)
        {
            Upgrade(entry.Currency);
            Debugger.Log(DebugCategory.Resources, $"Level {_level}: {entry.Currency}: {_data[entry.Currency]}");
        }
    }
    #endregion
    #region RESET
    public override void Reset()
    {
        _level = 1;

        foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
        {
            float value = _config.GetValue(currency, _level);
            _data[currency] = value;
            RaiseStatUpdateEvent(currency, value);
        }

        GameEventsManager.RaiseResourceReset();
    }

    protected override void RaiseStatUpdateEvent(CurrencyType stat, float value)
    {
        GameEventsManager.RaiseResourceStatUpdate(stat, value);
    }
    #endregion
}
