using System;
using System.Collections.Generic;

public abstract class Data<TEnum, TConfig> where TEnum : Enum
{
    protected Dictionary<TEnum, float> _data = new();
    protected TConfig _config;
    protected DebugCategory _debugCategory;
    protected int _level = 1;

    public int Level => _level;
    public IReadOnlyDictionary<TEnum, float> Snapshot => _data;

    // Initalise
    protected void Init(TConfig config, DebugCategory debugCategory)
    {
        _config = config;
        _debugCategory = debugCategory;

        // Initialising all stats to 0
        foreach (TEnum d in Enum.GetValues(typeof(TEnum)))
        {
            _data[d] = 0f;
        }

        // // Set initial values based on config
        // Reset();

        foreach (var stat in _data)
        {
            Debugger.Log(debugCategory, $"Level {_level}: {stat.Key}: {stat.Value}");
        }
    }

    public virtual float Get(TEnum stat) => _data[stat];

    public virtual void Set(TEnum stat, float value)
    {
        _data[stat] = value;
        RaiseStatUpdateEvent(stat, value);
    }

    public abstract void Reset();

    protected abstract void RaiseStatUpdateEvent(TEnum stat, float value);

    protected void LogAllStats()
    {
        foreach (var stat in _data)
            Debugger.Log(_debugCategory, $"Level {_level}: {stat.Key}: {stat.Value}");
    }
}