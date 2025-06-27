using System;
using UnityEngine;
using System.Collections.Generic;

public class CurrencyClicker : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] float _baseRateIncrease = 1.2f;

    // REFERENCES
    CurrencyStorage _storage;

    // VARIABLES
    Dictionary<CurrencyType, float> _clickRates = new();
    Dictionary<CurrencyType, int> _clickLevel = new();
    readonly float _defaultRate = 0.125f;

    // UNITY FUNCTIONS
    void Awake()
    {
        _storage = GetComponent<CurrencyStorage>();

        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _clickRates[type] = _defaultRate;
            _clickLevel[type] = 0;
        }
    }

    // GETTERS & SETTERS
    public float GetClickRate(CurrencyType type) => _clickRates[type];
    public float GetClickLevel(CurrencyType type) => _clickLevel[type];
    public void SetClickRate(CurrencyType type, float rate) => _clickRates[type] = rate;
    public void UpgradeClickRate(CurrencyType type)
    {
        _clickRates[type] *= _baseRateIncrease;
        _clickLevel[type]++;
    }

    public void Click(CurrencyType type)
    {
        _storage.Add(type, _clickRates[type]);
    }
}
