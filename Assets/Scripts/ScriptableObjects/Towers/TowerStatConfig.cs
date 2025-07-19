using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tower/Tower Defense Stat Config")]
public class TowerStatConfig : ScriptableObject
{
    public int MaxLevel;
    public TowerStatEntry[] Stats;
    public Dictionary<TowerStats, TowerStatEntry> _statLookup;

    void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetValue(TowerStats stat, int level = 0)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }

}