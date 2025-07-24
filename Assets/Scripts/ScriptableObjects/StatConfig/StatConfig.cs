using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class StatConfig<TEnum> : StatConfig
    where TEnum : Enum
{
    [Header("Base Parameters")]
    public int MaxLevel;
    public DebugCategory _debugCategory;

    public StatEntry<TEnum>[] Stats;
    protected Dictionary<TEnum, StatEntry<TEnum>> _statLookup;

    protected void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);
    public DebugCategory DebugCategory => _debugCategory;

    public float GetValue(TEnum stat, int level = 1)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }

    public float[] GetValuesInRange(TEnum stat, int startLevel, int endLevel)
    {
        int count = Mathf.Max(0, endLevel - startLevel + 1);
        float[] values = new float[count];
        for (int i = 0; i < count; i++)
        {
            int level = startLevel + i;
            values[i] = GetValue(stat, level);
        }
        return values;
    }

    public override void PrintValuesInRange(Enum stat, int start, int end)
    {
        if (stat is TEnum typeStat)
        {
            var values = GetValuesInRange(typeStat, start, end);

            for (int i = 0; i < values.Length; i++)
            {
                Debugger.Log(_debugCategory, $"[{typeStat}] Level {i}: {values[i]}");
            }
        }
        else
        {
            Debugger.LogError(_debugCategory, $"Invalid stat type for {typeof(TEnum).Name}");
        }
    }
}

public abstract class StatConfig : ScriptableObject
{
    public abstract void PrintValuesInRange(Enum stat, int start, int end);
}