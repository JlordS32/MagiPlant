// WARNING [DEPRECATED]

// using System;
// using UnityEngine;

// [Serializable]
// public class UpgradeEntry
// {
//     public string Name;
//     public UpgradeType Type;
//     public CurrencyType TargetCurrency;
//     public float Value;
//     public float BaseCost;
//     public float CostMultiplier;
//     public int Level;
//     public int MaxLevel;

//     [NonSerialized] public Action UpgradeLogic;

//     public void Upgrade()
//     {
//         if (Level < MaxLevel) Level++;
//     }

//     public float GetUpgradeValue() => Value * Level;
//     public float GetCost() => Mathf.Round(BaseCost * Mathf.Pow(CostMultiplier, Level));
// }