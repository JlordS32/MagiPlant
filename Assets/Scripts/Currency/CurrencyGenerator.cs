using UnityEngine;
using System.Collections.Generic;
using System;

public class CurrencyGenerator : MonoBehaviour, IUpgradableCurrency
{
    // PROPERTIES
    [SerializeField] float _interval = 1f;

    // REFERENCES
    CurrencyStorage _storage;

    // PRIVATE VARIABLES
    Dictionary<CurrencyType, float> _rates = new();
    float _timer = 0f;
    readonly float _defaultRate = 0.5f;

    void Awake()
    {
        _storage = GetComponent<CurrencyStorage>();

        // Initialise default rate on each currency
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _rates[type] = _defaultRate;
        }
    }

    // UNITY FUNCTIONS
    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _interval)
        {
            // Add value based on 
            foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
                _storage.Add(type, _rates[type]);

            // Reset timer
            _timer = 0f;
        }
    }

    // GETTERS & SETTERS
    public float GetRate(CurrencyType type) => _rates[type];

    public void ApplyUpgrade(UpgradeEntry upgrade, CurrencyType type)
    {
        if (upgrade.Type == UpgradeType.GenerateRate)
        {
            _rates[type] = upgrade.GetUpgradeValue();
            GameEventsManager.RaiseGenerateRateUpdated(type, _rates[type], upgrade.Level);
        }
    }

}