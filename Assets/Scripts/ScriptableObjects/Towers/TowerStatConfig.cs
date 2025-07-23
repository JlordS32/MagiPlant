using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tower/Tower Defense Stat Config")]
public class TowerStatConfig : ScriptableObject, IStatConfig<TowerStats>
{
    [Header("Base Parameters")]
    public int MaxLevel;
    public float BaseCost;

    [Header("Cost Parameters")]
    public CurrencyType[] CostType;
    public AnimationCurve CostUpgrade;
    public UpgradeOperation CostUpgradeOperation = UpgradeOperation.Multiply;

    [Header("Stat Entries")]
    public StatEntry<TowerStats>[] Stats;
    public Dictionary<TowerStats, StatEntry<TowerStats>> _statLookup;

    void InitLookup() => _statLookup ??= Stats.ToDictionary(stat => stat.Stat, stat => stat);

    public float GetCost(int level = 0)
    {
        float delta = CostUpgrade.Evaluate(level);

        return CostUpgradeOperation switch
        {
            UpgradeOperation.Add => BaseCost + delta,
            UpgradeOperation.Multiply => BaseCost * delta,
            UpgradeOperation.Exponent => Mathf.Pow(BaseCost, delta),
            _ => BaseCost
        };
    }

    public float GetValue(TowerStats stat, int level = 1)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }
}