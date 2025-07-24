using UnityEngine;

[CreateAssetMenu(menuName = "SO/Tower/Tower Defense Stat Config")]
public class TowerStatConfig : StatConfig<TowerStats>
{
    public float BaseCost;

    [Header("Cost Parameters")]
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