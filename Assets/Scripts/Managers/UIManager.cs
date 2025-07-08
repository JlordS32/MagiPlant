using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    CurrencyStorage _currencyStorage;
    CurrencyGenerator _currencyGenerator;

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
        _currencyGenerator = FindFirstObjectByType<CurrencyGenerator>();
        _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
    }

    // TODO: Make a scriptable object for each upgrade entry
    void Start()
    {
        UpgradeEntry entry = new()
        {
            Name = "Water Generator",
            Type = UpgradeType.GenerateRate,
            TargetCurrency = CurrencyType.Water,
            Value = 0.5f,
            BaseCost = 2f,
            CostMultiplier = 1.5f,
            MaxLevel = 10,
            Level = 1
        };

        entry.UpgradeLogic = () =>
        {
            if (_currencyStorage.Spend(CurrencyType.Sunlight, entry.GetCost()))
            {
                entry.Upgrade();
                _currencyGenerator.ApplyUpgrade(entry, CurrencyType.Water);
            }
        };

        _upgradePanel.Build(new List<UpgradeEntry> { entry });
        _upgradeConfigs.Add(entry);
    }

    void Update()
    {
        _timeTextUI.text = _timeManager.GetTimeString();
    }

    #region SUBSCRIPTIONS
    void OnEnable()
    {
        GameEventsManager.OnCurrencyUpdate += UpdateCurrencyUI;
        GameEventsManager.OnLevelUpUpdate += UpdateLevelUI;
        GameEventsManager.OnExpGainUpdate += UpdateExpText;
        GameEventsManager.OnGenerateRateUpdated += HandleGenerateRateUpdate;
    }

    void OnDisable()
    {
        GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
        GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
        GameEventsManager.OnExpGainUpdate -= UpdateExpText;
        GameEventsManager.OnGenerateRateUpdated -= HandleGenerateRateUpdate;

    }
    #endregion

    #region EVENT SUBSCRIBERS
    void UpdateCurrencyUI(CurrencyType type, float value)
    {
        _currencyTexts[type].text = $"{type}: {NumberFormatter.Format(value)}";
    }

    void UpdateLevelUI(int value)
    {
        _levelTextUI.text = $"Level: {NumberFormatter.Format(value)}";
    }

    void UpdateExpText(float currentExp, float requiredExp)
    {
        _expTextUI.text = $"EXP: {NumberFormatter.Format(currentExp)} / {NumberFormatter.Format(requiredExp)}";
    }

    void HandleGenerateRateUpdate(CurrencyType type, float rate, int level)
    {
        var entry = _upgradeConfigs.Find(e =>
        e.Type == UpgradeType.GenerateRate && e.TargetCurrency == type);

        _upgradePanel.RefreshUI(entry.Type, type, rate, level, entry.GetCost(), entry.MaxLevel);
    }
    #endregion
}