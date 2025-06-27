using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyStorage : MonoBehaviour
{
    // VARIABLES
    Dictionary<CurrencyType, Storage> _storage = new();

    // REFERENCES
    UIManager _uiManager;

    // UNITY FUNCTIONS
    void Awake()
    {
        _uiManager = FindFirstObjectByType<UIManager>();
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
            _storage[type] = new Storage();
    }

    // GETTERS & SETTER
    public void Add(CurrencyType type, float amount)
    {
        _storage[type].Add(amount);

        // UI Update
        _uiManager.UpdateCurrencyText(type, _storage[type].Value);
    }

    public bool Spend(CurrencyType type, float amount)
    {
        bool spent = _storage[type].Spend(amount);
        _uiManager.UpdateCurrencyText(type, _storage[type].Value);

        return spent;
    }

    public float Get(CurrencyType type) => _storage[type].Value;

    // RESET
    public void ResetAll()
    {
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
            _storage[type] = new Storage();
    }
}
