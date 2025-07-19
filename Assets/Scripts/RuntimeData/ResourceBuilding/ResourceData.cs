using System;
using System.Collections.Generic;

[Serializable]
public class ResourceData
{
    Dictionary<CurrencyType, float> _values = new();
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
            _values[currency] = 0f;
        }

        Reset();

        foreach (var entry in _values)
        {
            Debugger.Log(DebugCategory.Resources, $"Level {_level}: {entry.Key}: {entry.Value}");
        }
    }

    #region GETTERS & SETTERS
    public float Get(CurrencyType currency) => _values[currency];
    public void Set(CurrencyType currency, float value)
    {
        _values[currency] = value;
        GameEventsManager.RaiseResourceStatUpdate(currency, value); // assume you have this
    }
    #endregion
    #region UPGRADE
    public void Upgrade(CurrencyType currency)
    {
        float newValue = _config.GetValue(currency, _level);
        _values[currency] = newValue;
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
            Debugger.Log(DebugCategory.Resources, $"Level {_level}: {entry.Currency}: {_values[entry.Currency]}");
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
            _values[currency] = value;
            GameEventsManager.RaiseResourceStatUpdate(currency, value);
        }

        GameEventsManager.RaiseResourceReset();
    }
#endregion
}
