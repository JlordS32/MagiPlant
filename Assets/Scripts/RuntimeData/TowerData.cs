using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class TowerData
{
    Dictionary<TowerStats, float> _stats = new();

    TowerStatConfig _config;

    // Constructor
    public TowerData(TowerStatConfig config)
    {
        _config = config;

        // Initialising all stats to 0
        foreach (TowerStats stat in Enum.GetValues(typeof(TowerStats)))
        {
            _stats[stat] = 0f;
        }

        Reset();
    }

    public float Get(TowerStats stat) => _stats[stat];

    public void Set(TowerStats stat, float value) {
        _stats[stat] = value;
    }

    public void Reset()
    {
        _stats[TowerStats.Attack] = _config.Attack;
        _stats[TowerStats.Speed] = _config.Speed;
        _stats[TowerStats.Range] = _config.Range;
    }
}
