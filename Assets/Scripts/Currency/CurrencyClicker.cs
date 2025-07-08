using System;
using UnityEngine;
using System.Collections.Generic;

public class CurrencyClicker : MonoBehaviour, IUpgradableCurrency
{
    // PROPERTIES
    [SerializeField] float _baseRateIncrease = 1.2f;

    // REFERENCES
    CurrencyStorage _storage;

    // VARIABLES
    Dictionary<CurrencyType, float> _clicks = new();
    readonly float _defaultRate = 0.125f;

    // UNITY FUNCTIONS
    void Awake()
    {
        _storage = GetComponent<CurrencyStorage>();

        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _clicks[type] = _defaultRate;
        }
    }

    // GETTERS & SETTERS
    public float GetClickRate(CurrencyType type) => _clicks[type];


    public void Click(CurrencyType type)
    {
        _storage.Add(type, _clicks[type]);
    }

    public void ApplyUpgrade(UpgradeEntry upgrade, CurrencyType type)
    {
        if (upgrade.Type == UpgradeType.ClickRate)
        {
            _clicks[type] = upgrade.GetUpgradeValue();
            GameEventsManager.RaiseClickRateUpdated(type, _clicks[type], upgrade.Level);
        }
    }
}
