using UnityEngine;
using System.Collections.Generic;
using System;

public class CurrencyGenerator : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] float _interval = 1f;
    [SerializeField] float _baseRateIncrease = 1.5f;

    // REFERENCES
    CurrencyStorage _storage;

    // PRIVATE VARIABLES
    Dictionary<CurrencyType, float> _rates = new();
    Dictionary<CurrencyType, int> _rateLevel = new();
    float _timer = 0f;
    readonly float _defaultRate = 0.5f;

    void Awake()
    {
        _storage = GetComponent<CurrencyStorage>();

        // Initialise default rate on each currency
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _rates[type] = _defaultRate;
            _rateLevel[type] = 0;
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
    public int GetRateLevel(CurrencyType type) => _rateLevel[type];
    public void SetRate(CurrencyType type, float rate) => _rates[type] = rate;
    
    public void UpgradeRate(CurrencyType type)
    {
        _rates[type] *= _baseRateIncrease;
        _rateLevel[type]++;
    }
}