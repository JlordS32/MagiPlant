using System;
using System.Collections.Generic;

public interface IStatData
{
    public int Level { get; }
    event Action<int> OnLevelChanged;
    Dictionary<string, float> GetStats();
    string GetDescription();
    float GetBaseValue(string stat);
    float GetMaxLevelValue(string stat);
    int GetMaxLevel();
}
