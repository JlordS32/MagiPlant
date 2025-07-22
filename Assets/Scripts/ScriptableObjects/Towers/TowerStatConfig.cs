using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tower/Tower Defense Stat Config")]
public class TowerStatConfig : ScriptableObject
{
    public int MaxLevel;
    public float BaseCost;
    public CurrencyType[] CostType;
    public AnimationCurve CostUpgrade;
    public UpgradeOperation CostUpgradeOperation = UpgradeOperation.Multiply;
    public TowerStatEntry[] Stats;
    public Dictionary<TowerStats, TowerStatEntry> _statLookup;

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

    public float GetValue(TowerStats stat, int level = 0)
    {
        InitLookup();
        return _statLookup.TryGetValue(stat, out var entry) ? entry.GetValue(level) : 0f;
    }
}