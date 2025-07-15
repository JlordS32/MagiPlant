using UnityEngine;
using System.Collections.Generic;
using System;

public class CurrencyGenerator : MonoBehaviour, IUpgradableCurrency, IPhaseListener
{
    // PROPERTIES
    [SerializeField] float _interval = 1f;

    // REFERENCES
    CurrencyStorage _storage;

    // PRIVATE VARIABLES
    Dictionary<CurrencyType, float> _rates = new();
    bool _canGenerate = true;
    float _timer = 0f;
    readonly float _defaultRate = 0.5f;

    #region PHASE LISTENER
    void OnEnable()
    {
        PhaseService.Register(this);
        OnPhaseChanged(PhaseService.Current);
    }
    void OnDisable()
    {
        PhaseService.Unregister(this);
    }

    public void OnPhaseChanged(GamePhase phase)
    {
        _canGenerate = phase == GamePhase.Day;

        if (_canGenerate)
            _timer = 0f;

        Debug.LogWarning($"CurrencyGenerator phase changed: {phase}, enabled: {_canGenerate}");
    }

    #endregion

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
        if (!_canGenerate || _storage == null) return;
        
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