using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System;


public class UIManager : MonoBehaviour
{
    // PROPERTIES
    [Header("Sidebar UI")]
    [SerializeField] UpgradePanelUI _upgradePanel;

    [Header("Currency UI")]
    [SerializeField] TextMeshProUGUI _waterTextUI;
    [SerializeField] TextMeshProUGUI _sunlightTextUI;

    [Header("Level UI")]
    [SerializeField] TextMeshProUGUI _levelTextUI;
    [SerializeField] TextMeshProUGUI _expTextUI;

    [Header("Upgrades Config")]
    [SerializeField] List<UpgradeEntry> _upgradeConfigs;

    // REFERENCES
    UpgradeManager _upgradeManager;
    CurrencyGenerator _currencyGenerator;
    CurrencyClicker _currencyClicker;


    // VARIABLES
    Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new();
    List<UpgradeEntry> _entries = new();

    void Awake()
    {
        _currencyTexts[CurrencyType.Water] = _waterTextUI;
        _currencyTexts[CurrencyType.Sunlight] = _sunlightTextUI;

        _upgradeManager = FindFirstObjectByType<UpgradeManager>();
        _currencyGenerator = FindFirstObjectByType<CurrencyGenerator>();
        _currencyClicker = FindAnyObjectByType<CurrencyClicker>();
    }

    void Start()
    {
        AddUpgrade("Water Generation",
            () => _upgradeManager.Upgrade(_currencyGenerator.GetRateLevel(CurrencyType.Water), () => _currencyGenerator.UpgradeRate(CurrencyType.Water)),
            () => $"{NumberFormatter.Format(_currencyGenerator.GetRate(CurrencyType.Water))}/s",
            () => $"Cost: {NumberFormatter.Format(_upgradeManager.GetCost(_currencyGenerator.GetRateLevel(CurrencyType.Water)))} suns",
            () => $"Level: {_currencyGenerator.GetRateLevel(CurrencyType.Water)}");

        _upgradePanel.Build(_entries);
    }

    public void AddUpgrade(string name, UnityAction logic, Func<string> getRate, Func<string> getButtonLabel, Func<string> getLevel)
    {
        _entries.Add(new UpgradeEntry
        {
            Name = name,
            UpgradeLogic = logic,
            GetRate = getRate,
            GetButtonLabel = getButtonLabel,
            GetLevel = getLevel
        });
    }

    public void UpdateCurrencyText(CurrencyType type, float value)
    {
        if (_currencyTexts.TryGetValue(type, out var uiText))
            uiText.text = $"{type}: {NumberFormatter.Format(value)}";
    }

    public void UpdateLevelText(int value)
    {
        _levelTextUI.text = $"Level: {NumberFormatter.Format(value)}";
    }

    public void UpdateExpText(float value, float cap)
    {
        _expTextUI.text = $"EXP: {NumberFormatter.Format(value)} / {NumberFormatter.Format(cap)}";
    }
}