using UnityEngine;
using System;

public class UpgradeManager : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] float _baseCost = 2f;
    [SerializeField] float _costMultiplier = 1.5f;

    // REFERENCES
    CurrencyStorage _storage;

    // UNITY FUNCTIONS
    void Awake() => _storage = GetComponent<CurrencyStorage>();

    // CUSTOM FUNCTIONS HERE
    // ---------------------
    public float GetCost(int level) => Mathf.Round(_baseCost * Mathf.Pow(_costMultiplier, level));

    public bool Upgrade(int level, Action upgradeAction)
    {
        float cost = GetCost(level);

        if (_storage.Spend(CurrencyType.Sunlight, cost))
        {
            upgradeAction?.Invoke();
            return true;
        }

        return false;
    }
}
