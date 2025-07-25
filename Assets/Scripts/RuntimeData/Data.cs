using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Data<TEnum, TConfig> : IStatData
    where TEnum : Enum
    where TConfig : StatConfig<TEnum>
{
    public event Action<int> OnLevelChanged;

    protected Dictionary<TEnum, float> _data = new();
    protected TConfig _config;
    protected int _level = 1;

    // GETTERS & SETTERS
    public int Level => _level;
    public IReadOnlyDictionary<TEnum, float> Snapshot => _data;
    public Dictionary<string, float> GetStats() => _data.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value);
    public virtual float Get(TEnum stat) => _data[stat];
    public virtual void Set(TEnum stat, float value)
    {
        _data[stat] = value;
        RaiseStatUpdateEvent(stat, value);
    }

    // Initalise
    protected void Init(TConfig config)
    {
        _config = config;

        // Initialising all stats to 0
        foreach (TEnum d in Enum.GetValues(typeof(TEnum)))
        {
            _data[d] = 0f;
        }

        // // Set initial values based on config
        Reset();

        foreach (var stat in _data)
        {
            Debugger.Log(_config.DebugCategory, $"Level {_level}: {stat.Key}: {stat.Value}");
        }
    }

    // Upgrade logic
    public virtual void TryUpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            OnUpgradeBlocked("Max level reached.");
            return;
        }

        if (!CanUpgrade())
        {
            OnUpgradeBlocked("Upgrade blocked by conditions.");
            return;
        }

        _level++;
        RaiseLevelChanged(_level);

        foreach (var entry in _config.Stats)
        {
            float newValue = Mathf.FloorToInt(_config.GetValue(entry.Stat, _level));
            _data[entry.Stat] = newValue;
            RaiseStatUpdateEvent(entry.Stat, newValue);
        }

        LogAllStats();
        PostUpgrade();
    }

    protected virtual void OnUpgradeBlocked(string reason)
    {
        Debugger.LogWarning(_config.DebugCategory, reason);
    }

    protected virtual bool CanUpgrade() => true;
    protected virtual void PostUpgrade() { }

    public virtual void Reset()
    {
        foreach (TEnum stat in Enum.GetValues(typeof(TEnum)))
            _data[stat] = Mathf.FloorToInt(_config.GetBaseValue(stat));

        _level = 1;
        RaiseResetUpdateEvent();
    }

    // TO BE IMPLEMENTED ON INHERITING CLASSES
    protected abstract void RaiseStatUpdateEvent(TEnum stat, float value);
    protected abstract void RaiseResetUpdateEvent();
    protected void RaiseLevelChanged(int level) => OnLevelChanged?.Invoke(level);

    // DEBUG
    public void LogAllStats()
    {
        foreach (var stat in _data)
            Debugger.Log(_config.DebugCategory, $"Level {_level}: {stat.Key}: {stat.Value}");
    }
}