using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//TODO[JAYLOU]: Update the stat config so it's more configurable like Resource and Tower.
//TODO: Centralised required exp in this config using animation curve.
[CreateAssetMenu(menuName = "SO/Player/Player Stat Config")]
public class PlayerStatConfig : ScriptableObject
{
    public int MaxLevel;

    [Header("Stat Entries")]
    public StatEntry<PlayerStats>[] Stats;
    public Dictionary<PlayerStats, StatEntry<PlayerStats>> _statLookup;

    [Header("EXP Settings")]
    public float BaseExp = 10f;
    public AnimationCurve ExpCurve;
    public UpgradeOperation ExpOperation = UpgradeOperation.Multiply;

    void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetValue(PlayerStats stat, int level = 0)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }

    public float GetRequiredExp(int level)
    {
        float delta = ExpCurve?.Evaluate(level) ?? 1f;
        
        return ExpOperation switch
        {
            UpgradeOperation.Add => BaseExp + delta,
            UpgradeOperation.Multiply => BaseExp * delta,
            UpgradeOperation.Exponent => Mathf.Pow(BaseExp, delta),
            _ => BaseExp
        };
    }
}
