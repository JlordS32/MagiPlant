using System;
using System.Collections.Generic;

public interface IStatData
{
    public int Level { get; }
    Dictionary<string, float> GetStats();
    event Action<int> OnLevelChanged;
    float GetBaseValue(string stat);
    float GetMaxLevelValue(string stat);
    int GetMaxLevel();
}
