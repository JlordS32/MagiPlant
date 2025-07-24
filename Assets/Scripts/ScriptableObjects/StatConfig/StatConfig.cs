using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class StatConfig<TEnum> : ScriptableObject
    where TEnum : Enum
{
    [Header("Base Parameters")]
    public int MaxLevel;
    public StatEntry<TEnum>[] Stats;
    protected Dictionary<TEnum, StatEntry<TEnum>> _statLookup;

    protected void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetValue(TEnum stat, int level = 1)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }

    public float[] GetValuesInRange(TEnum stat, int startLevel = 1, int endLevel = 100)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry)
            ? entry.GetValuesInRange(startLevel, endLevel)
            : new float[0];
    }

    public void PrintValuesInRange(TEnum stat, int startLevel = 1, int endLevel = 100)
    {
        var values = GetValuesInRange(stat, startLevel, endLevel);
        for (int i = 0; i < values.Length; i++)
        {
            Debug.Log($"[{stat}] Level {startLevel + i}: {values[i]}");
        }
    }
}
