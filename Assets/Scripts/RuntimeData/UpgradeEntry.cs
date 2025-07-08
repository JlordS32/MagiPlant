using System;
using UnityEngine.Events;

[System.Serializable]
public class UpgradeEntry
{
    public UpgradeType Type;
    public float Value;
    public int Cost;
    public int Level;
    public int MaxLevel;

    public void Upgrade()
    {
        if (Level < MaxLevel) Level++;
    }

    public float GetUpgradeValue() => Value * Level;
}