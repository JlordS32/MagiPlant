using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    TimeManager _timeManager;

    // VARIABLES
    Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new();
    Dictionary<PlayerStats, TextMeshProUGUI> _playerStatTexts = new();

    // IMPORTANT: ASSIGN UI COMPONENTS HERE USING VIA HASHMAP
    void Awake()
    {
        // Currency
        _currencyTexts[CurrencyType.Water] = _waterTextUI;
        _currencyTexts[CurrencyType.Sunlight] = _sunlightTextUI;

        // Player
        _playerStatTexts[PlayerStats.Level] = _levelTextUI;
        _playerStatTexts[PlayerStats.EXP] = _expTextUI;

        // References
        _timeManager = FindFirstObjectByType<TimeManager>();
    }

    void OnEnable()
    {
        GameEventsManager.OnCurrencyUpdate += UpdateCurrencyUI;
        GameEventsManager.OnLevelUpUpdate += UpdateLevelUI;
        GameEventsManager.OnExpGainUpdate += UpdateExpText;
    }

    void OnDisable()
    {
        GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
        GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
        GameEventsManager.OnExpGainUpdate -= UpdateExpText;
    }

    void UpdateCurrencyUI(CurrencyType type, float value)
    {
        _currencyTexts[type].text = $"{type}: {NumberFormatter.Format(value)}";
    }

    public void UpdateLevelUI(int value)
    {
        _levelTextUI.text = $"Level: {NumberFormatter.Format(value)}";
    }

    public void UpdateExpText(float currentExp, float requiredExp)
    {
        _expTextUI.text = $"EXP: {NumberFormatter.Format(currentExp)} / {NumberFormatter.Format(requiredExp)}";
    }

    void Update()
    {
        _timeTextUI.text = _timeManager.GetTimeString();
    }
}