using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "SO/Player/Player Stat Config")]
public class PlayerStatConfig : ScriptableObject, IStatConfig<PlayerStats>
{
    public int MaxLevel;

    [Header("Stat Entries")]
    public StatEntry<PlayerStats>[] Stats;
    public Dictionary<PlayerStats, StatEntry<PlayerStats>> _statLookup;

    void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetValue(PlayerStats stat, int level = 1)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }

    public float GetRequiredExp(int level)
    {
        return GetValue(PlayerStats.EXP, level);
    }
}
