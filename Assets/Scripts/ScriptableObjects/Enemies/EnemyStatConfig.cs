using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO[JAYLOU]: Update the stat config so it's more configurable like Resource and Tower.
[CreateAssetMenu(menuName = "SO/Enemies/Enemy Stat Config")]
public class EnemyStatConfig : ScriptableObject
{
    public int MaxLevel;
    public float BaseCost;
    public EnemyStatEntry[] Stats;
    public Dictionary<EnemyStats, EnemyStatEntry> _statLookup;

    void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetValue(EnemyStats stat, int level = 0)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }
}
