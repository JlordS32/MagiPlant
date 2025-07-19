using System;
using UnityEngine;

[Serializable]
public class TowerStatEntry
{
    public string Name;
    public TowerStats Stat;
    public float BaseValue;
    public AnimationCurve UpgradeCurve;
    public UpgradeOperation UpgradeOperation = UpgradeOperation.Add;

    public float GetValue(int level)
    {
        float delta = UpgradeCurve.Evaluate(level);
        return UpgradeOperation switch
        {
            UpgradeOperation.Add => BaseValue + delta,
            UpgradeOperation.Multiply => BaseValue * delta,
            UpgradeOperation.Exponent => Mathf.Pow(BaseValue, delta),
            _ => BaseValue,
        };
    }
}