using System;
using System.Collections.Generic;

public interface IStatData
{
    int Level { get; }
    Dictionary<string, float> GetStats();
    public event Action<int> OnLevelChanged;
}
