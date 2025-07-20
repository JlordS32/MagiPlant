using System;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyStorage : MonoBehaviour
{
    public static CurrencyStorage Instance { get; private set; }

    // VARIABLES
    Dictionary<CurrencyType, Storage> _storage = new();
    readonly float _initialValue = 10f;

    // UNITY FUNCTIONS
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of CurrencyStorage detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _storage[type] = new Storage(_initialValue);
        }
    }

    // GETTERS & SETTER
    public void Add(CurrencyType type, float amount)
    {
        _storage[type].Add(amount);
        GameEventsManager.RaiseCurrencyUpdate(type, _storage[type].Value);
    }

    public bool Spend(CurrencyType type, float amount)
    {
        bool spent = _storage[type].Spend(amount);
        GameEventsManager.RaiseCurrencyUpdate(type, _storage[type].Value);

        return spent;
    }

    public bool CanAfford(CurrencyType type, float amount)
    {
        return _storage[type].Value >= amount;
    }

    public float Get(CurrencyType type) => _storage[type].Value;

    // RESET
    public void ResetAll()
    {
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
            _storage[type] = new Storage();

        GameEventsManager.RaiseCurrencyReset();
    }
}
