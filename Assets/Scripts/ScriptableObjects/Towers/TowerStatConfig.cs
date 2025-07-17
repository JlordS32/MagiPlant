using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tower/Tower Defense Stat Config")]
public class TowerStatConfig : ScriptableObject
{
    public TowerStatEntry[] Stats;
    public Dictionary<TowerStats, float> _statLookup;

    public float GetValue(TowerStats stat)
    {
        _statLookup ??= Stats.ToDictionary(entry => entry.Stat, entry => entry.Value);
        return _statLookup.TryGetValue(stat, out var value) ? value : 0f;
    }
}
