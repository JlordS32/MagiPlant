using System;
using UnityEngine;

[Serializable]
public class StatEntry<TEnum> where TEnum : Enum
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
            UpgradeOperation.Add => Mathf.FloorToInt(BaseValue + delta),
            UpgradeOperation.Multiply => Mathf.FloorToInt(BaseValue * delta),
            UpgradeOperation.Exponent => Mathf.FloorToInt(Mathf.Pow(BaseValue, delta)),
            _ => BaseValue,
        };
    }
}