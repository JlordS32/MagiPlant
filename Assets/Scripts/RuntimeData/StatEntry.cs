using System;
using UnityEngine;

[Serializable]
public abstract class StatEntry<TEnum> where TEnum : Enum
{
    public string Name;
    public TEnum Stat;
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