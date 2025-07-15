using System;
using UnityEngine;
using System.Collections.Generic;

public class CurrencyClicker : MonoBehaviour, IUpgradableCurrency, IPhaseListener
{
    // REFERENCES
    CurrencyStorage _storage;

    // VARIABLES
    Dictionary<CurrencyType, float> _clickRate = new();
    readonly float _defaultRate = 0.125f;
    bool _canClick = true;

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
        _canClick = phase == GamePhase.Day;
    }
    #endregion

    // UNITY FUNCTIONS
    void Awake()
    {
        _storage = GetComponent<CurrencyStorage>();

        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _clickRate[type] = _defaultRate;
        }
    }

    // GETTERS & SETTERS
    public float GetClickRate(CurrencyType type) => _clickRate[type];


    public void Click(CurrencyType type)
    {
        if (!_canClick) return;
        _storage.Add(type, _clickRate[type]);
    }

    public void ApplyUpgrade(UpgradeEntry upgrade, CurrencyType type)
    {
        if (upgrade.Type == UpgradeType.ClickRate)
        {
            _clickRate[type] = upgrade.GetUpgradeValue();
            GameEventsManager.RaiseClickRateUpdated(type, _clickRate[type], upgrade.Level);
        }
    }
}
