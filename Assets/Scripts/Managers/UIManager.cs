using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System;

// TODO: After refactoring upgrade system. Link it up to the upgrade UI component.
///
/// Suggestions below:
/// 
/// After finishing refactoring the upgrade logic. Look at UpgradePanelUI.cs and UpgradeUI.cs
/// Our goal here is to add the upgrade callback to the upgradeUI button and make sure it builds
/// on start of UpgradePanelUI
/// 
///
public class UIManager : MonoBehaviour
{
    // PROPERTIES
    [Header("General")]
    [SerializeField] TextMeshProUGUI _timeTextUI;

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
    CurrencyStorage _currencyStorage;
    TimeManager _timeManager;

    // VARIABLES
    Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new();
    List<UpgradeEntry> _entries = new();

    float _water;

    void Awake()
    {
        _currencyTexts[CurrencyType.Water] = _waterTextUI;
        _currencyTexts[CurrencyType.Sunlight] = _sunlightTextUI;

        _upgradeManager = FindFirstObjectByType<UpgradeManager>();
        _currencyGenerator = FindFirstObjectByType<CurrencyGenerator>();
        _timeManager = FindFirstObjectByType<TimeManager>();
        _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
    }

    void OnEnable()
    {
        GameEventsManager.OnCurrencyUpdate += CurrentRate;
    }

    void CurrentRate(Dictionary<CurrencyType, Storage> storage)
    {
        _water = storage[CurrencyType.Water].Value;
    }

    void Start()
    {
    }

    void Update()
    {
        _timeTextUI.text = _timeManager.GetTimeString();
    }

    public void AddUpgrade(string name, UnityAction logic, Func<string> getRate, Func<string> getButtonLabel, Func<string> getLevel)
    {

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