using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TODO: Make a dynamic UI for buildings just like the upgrade.
public class UIManager : MonoBehaviour
{
    // PROPERTIES
    [Header("General")]
    [SerializeField] TextMeshProUGUI _timeTextUI;

    [Header("Sidebar UI")]
    [SerializeField] SidePanelUI _sidePanel;
    [SerializeField] TowerConfig _towerDefenses;

    [Header("Currency UI")]
    [SerializeField] TextMeshProUGUI _waterTextUI;
    [SerializeField] TextMeshProUGUI _sunlightTextUI;

    [Header("Level UI")]
    [SerializeField] TextMeshProUGUI _levelTextUI;
    [SerializeField] TextMeshProUGUI _expTextUI;

    // REFERENCES
    TimeManager _timeManager;
    BuildManager _buildManager;

    // VARIABLES
    Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new();
    Dictionary<PlayerStats, TextMeshProUGUI> _playerStatTexts = new();
    List<DefenseEntry> _defenseEntries;

    // IMPORTANT: ASSIGN UI COMPONENTS HERE USING VIA HASHMAP
    void Awake()
    {
        _defenseEntries = new List<DefenseEntry>();

        // Currency
        _currencyTexts[CurrencyType.Water] = _waterTextUI;
        _currencyTexts[CurrencyType.Sunlight] = _sunlightTextUI;

        // Player
        _playerStatTexts[PlayerStats.Level] = _levelTextUI;
        _playerStatTexts[PlayerStats.EXP] = _expTextUI;

        // References
        _timeManager = FindFirstObjectByType<TimeManager>();
        _buildManager = FindFirstObjectByType<BuildManager>();
    }

    // TODO: Make a scriptable object for each upgrade entry
    void Start()
    {
        foreach (var defenses in _towerDefenses.DefenseEntry)
        {
            // Manually assign the function to build for each entry.
            defenses.UpgradeLogic = () =>
            {
                _buildManager.SelectPrefab(defenses.DefensePrefab);
            };

            _defenseEntries.Add(defenses);
        }

        _sidePanel.Build(_defenseEntries);
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
    }

    void OnDisable()
    {
        GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
        GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
        GameEventsManager.OnExpGainUpdate -= UpdateExpText;
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
    #endregion
}