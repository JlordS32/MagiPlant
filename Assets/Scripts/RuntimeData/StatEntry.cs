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
            UpgradeOperation.Add => BaseValue + delta,
            UpgradeOperation.Multiply => BaseValue * delta,
            UpgradeOperation.Exponent => Mathf.Pow(BaseValue, delta),
            _ => BaseValue,
        };
    }

    public float[] GetValuesInRange(int startLevel, int endLevel)
    {
        int count = Mathf.Max(0, endLevel - startLevel + 1);
        float[] values = new float[count];
        for (int i = 0; i < count; i++)
        {
            int level = startLevel + i;
            values[i] = GetValue(level);
        }
        return values;
    }
}