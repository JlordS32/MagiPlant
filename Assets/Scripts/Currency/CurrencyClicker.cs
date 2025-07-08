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
    Dictionary<CurrencyType, int> _clickLevel = new();
    readonly float _defaultRate = 0.125f;

    // UNITY FUNCTIONS
    void Awake()
    {
        _storage = GetComponent<CurrencyStorage>();

        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _clicks[type] = _defaultRate;
            _clickLevel[type] = 0;
        }
    }

    // GETTERS & SETTERS
    public float GetClickRate(CurrencyType type) => _clicks[type];
    public float GetClickLevel(CurrencyType type) => _clickLevel[type];

    public void SetClickRate(CurrencyType type, float rate)
    {
        _clicks[type] = rate;
        GameEventsManager.RaiseClickRateUpdated(type, _clicks[type], _clickLevel[type]);
    }
    public void UpgradeClickRate(CurrencyType type)
    {
        _clicks[type] *= _baseRateIncrease;
        _clickLevel[type]++;
        GameEventsManager.RaiseClickRateUpdated(type, _clicks[type], _clickLevel[type]);
    }

    public void Click(CurrencyType type)
    {
        _storage.Add(type, _clicks[type]);
    }

    public void ApplyUpgrade(UpgradeEntry upgrade, CurrencyType type)
    {
        if (upgrade.Type == UpgradeType.ClickRate)
        {
            UpgradeClickRate(type);
        }
    }
}
