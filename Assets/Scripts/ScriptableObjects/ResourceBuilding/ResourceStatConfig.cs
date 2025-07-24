using UnityEngine;

[CreateAssetMenu(menuName = "SO/Resources/Resource Stat Config")]
public class ResourceStatConfig : StatConfig<CurrencyType>
{
    public float Interval;

    [Header ("Cost Parameters")]
    public float BaseCost;
    public CurrencyType[] CostType;
    public AnimationCurve CostUpgrade;
    public UpgradeOperation CostUpgradeOperation = UpgradeOperation.Multiply;

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
}