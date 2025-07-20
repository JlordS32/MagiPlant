using System;
using System.Collections.Generic;

[Serializable]
public class ResourceData
{
    Dictionary<CurrencyType, float> _rates = new();
    ResourceStatConfig _config;

    int _level = 1;
    public int Level => _level;

    // Constructor
    public ResourceData(ResourceStatConfig config)
    {
        _config = config;

        // Initialise values
        foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
        {
            _rates[currency] = 0f;
        }

        Reset();

        foreach (var entry in _rates)
        {
            Debugger.Log(DebugCategory.Resources, $"Level {_level}: {entry.Key}: {entry.Value}");
        }
    }

    #region GETTERS & SETTERS
    public IReadOnlyDictionary<CurrencyType, float> Snapshot => _rates;
    public float Get(CurrencyType currency) => _rates[currency];
    public void Set(CurrencyType currency, float value)
    {
        _rates[currency] = value;
        GameEventsManager.RaiseResourceStatUpdate(currency, value); // assume you have this
    }
    #endregion
    #region UPGRADE
    public void Upgrade(CurrencyType currency)
    {
        float newValue = _config.GetValue(currency, _level);
        _rates[currency] = newValue;
        GameEventsManager.RaiseResourceStatUpdate(currency, newValue);
    }

    public void UpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            Debugger.LogWarning(DebugCategory.Resources, "Attempting to upgrade resources beyond max level.");
            return;
        }

        _level++;

        foreach (var entry in _config.Resources)
        {
            Upgrade(entry.Currency);
            Debugger.Log(DebugCategory.Resources, $"Level {_level}: {entry.Currency}: {_rates[entry.Currency]}");
        }
    }
    #endregion
    #region RESET
    public void Reset()
    {
        _level = 1;

        foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
        {
            float value = _config.GetValue(currency, 0);
            _rates[currency] = value;
            GameEventsManager.RaiseResourceStatUpdate(currency, value);
        }

        GameEventsManager.RaiseResourceReset();
    }
#endregion
}
