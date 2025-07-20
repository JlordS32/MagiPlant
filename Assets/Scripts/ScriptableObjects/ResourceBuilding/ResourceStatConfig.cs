using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Resources/Resource Stat Config")]
public class ResourceStatConfig : ScriptableObject
{
    public int MaxLevel;
    public float Interval;
    public float BaseCost;
    public CurrencyType[] CostType;
    public AnimationCurve CostUpgrade;
    public UpgradeOperation CostUpgradeOperation = UpgradeOperation.Multiply;
    public ResourceStatEntry[] Resources;
    public Dictionary<CurrencyType, ResourceStatEntry> _statLookup;

    void InitLookup() => _statLookup ??= Resources.ToDictionary(currency => currency.Currency, currency => currency);

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

    public float GetValue(CurrencyType currency, int level = 0)
    {
        InitLookup();
        return _statLookup.TryGetValue(currency, out var entry) ? entry.GetValue(level) : 0f;
    }
}