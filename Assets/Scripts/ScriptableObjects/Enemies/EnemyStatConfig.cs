using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO[JAYLOU]: Update the stat config so it's more configurable like Resource and Tower.
[CreateAssetMenu(menuName = "SO/Enemies/Enemy Stat Config")]
public class EnemyStatConfig : ScriptableObject, IStatConfig<EnemyStats>
{
    public int MaxLevel;
    public float BaseCost;
    public StatEntry<EnemyStats>[] Stats;
    public Dictionary<EnemyStats, StatEntry<EnemyStats>> _statLookup;

    void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetValue(EnemyStats stat, int level = 1)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }
}
