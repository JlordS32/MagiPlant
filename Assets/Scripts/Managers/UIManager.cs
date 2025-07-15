using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
// using TMPro;

// public class UIManager : MonoBehaviour
// {
//     // PROPERTIES
//     [Header("General")]
//     [SerializeField] TextMeshProUGUI _timeTextUI;

//     [Header("Sidebar UI")]
//     [SerializeField] SidePanelUI _sidePanel;
//     [SerializeField] TowerConfig _towerDefenses;

//     [Header("Currency UI")]
//     [SerializeField] TextMeshProUGUI _waterTextUI;
//     [SerializeField] TextMeshProUGUI _sunlightTextUI;

//     [Header("Level UI")]
//     [SerializeField] TextMeshProUGUI _levelTextUI;
//     [SerializeField] TextMeshProUGUI _expTextUI;

//     // REFERENCES
//     TimeManager _timeManager;
//     BuildManager _buildManager;
//     CurrencyStorage _currencyStorage;

//     // VARIABLES
//     Dictionary<CurrencyType, TextMeshProUGUI> _currencyTexts = new();
//     Dictionary<PlayerStats, TextMeshProUGUI> _playerStatTexts = new();
//     List<DefenseEntry> _defenseEntries;

//     // IMPORTANT: ASSIGN UI COMPONENTS HERE USING VIA HASHMAP
//     void Awake()
//     {
//         _defenseEntries = new List<DefenseEntry>();

//         // Currency
//         _currencyTexts[CurrencyType.Water] = _waterTextUI;
//         _currencyTexts[CurrencyType.Sunlight] = _sunlightTextUI;

//         // Player
//         _playerStatTexts[PlayerStats.Level] = _levelTextUI;
//         _playerStatTexts[PlayerStats.EXP] = _expTextUI;

//         // References
//         _timeManager = FindFirstObjectByType<TimeManager>();
//         _buildManager = FindFirstObjectByType<BuildManager>();
//         _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
//     }

//     void Start()
//     {
//         foreach (var defenses in _towerDefenses.DefenseEntry)
//         {
//             // Manually assign the function to build for each entry.
//             defenses.UpgradeLogic = () =>
//             {
//                 if (_currencyStorage.Spend(CurrencyType.Sunlight, defenses.Cost))
//                 {
//                     _buildManager.SelectPrefab(defenses.DefensePrefab);
//                 } else
//                 {
//                     Debug.LogWarning($"Not enough sunlight to build {defenses.DefensePrefab.name}");
//                 }
//             };

//             _defenseEntries.Add(defenses);
//         }

//         _sidePanel.Build(_defenseEntries);
//     }

//     void Update()
//     {
//         _timeTextUI.text = _timeManager.GetTimeString();
//     }

//     #region SUBSCRIPTIONS
//     void OnEnable()
//     {
//         GameEventsManager.OnCurrencyUpdate += UpdateCurrencyUI;
//         GameEventsManager.OnLevelUpUpdate += UpdateLevelUI;
//         GameEventsManager.OnExpGainUpdate += UpdateExpText;
//     }

//     void OnDisable()
//     {
//         GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
//         GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
//         GameEventsManager.OnExpGainUpdate -= UpdateExpText;
//     }
//     #endregion

//     #region EVENT SUBSCRIBERS
//     void UpdateCurrencyUI(CurrencyType type, float value)
//     {
//         _currencyTexts[type].text = $"{type}: {NumberFormatter.Format(value)}";
//     }

//     void UpdateLevelUI(int value)
//     {
//         _levelTextUI.text = $"Level: {NumberFormatter.Format(value)}";
//     }

//     void UpdateExpText(float currentExp, float requiredExp)
//     {
//         _expTextUI.text = $"EXP: {NumberFormatter.Format(currentExp)} / {NumberFormatter.Format(requiredExp)}";
//     }
//     #endregion
// }

public class UIManager : MonoBehaviour
{
    [Header("UI Toolkit")]
    [SerializeField] UIDocument uiDocument;
    [SerializeField] VisualTreeAsset towerEntryTemplate;

    [Header("Tower Data")]
    [SerializeField] TowerConfig _towerDefenses;

    private VisualElement root;
    private VisualElement panel;
    private Button collapseButton;

    private List<DefenseEntry> _defenseEntries = new();


    private CurrencyStorage _currencyStorage;
    private BuildManager _buildManager;

    void Awake()
    {
        _currencyStorage = FindObjectOfType<CurrencyStorage>();
        _buildManager = FindObjectOfType<BuildManager>();
    }

    void Start()
    {
        root = uiDocument.rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        collapseButton = root.Q<Button>("CollapseButton");

        panel.style.display = DisplayStyle.None;
        collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
        collapseButton.style.left = new StyleLength(new Length(2, LengthUnit.Percent));

        collapseButton.RegisterCallback<ClickEvent>(OnCollapseClicked);

        BuildSidebar();
    }

    void OnCollapseClicked(ClickEvent evt)
    {
        panel.style.display = panel.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        if (panel.style.display == DisplayStyle.Flex)
        {
            collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
            collapseButton.style.left = new StyleLength(new Length(18.5f, LengthUnit.Percent));
            Debug.Log("Open");
        }
        else
        {
            collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
            collapseButton.style.left = new StyleLength(new Length(2, LengthUnit.Percent));
            Debug.Log("Close");

        }
    }

    private void BuildSidebar()
    {
        foreach (var entry in _towerDefenses.DefenseEntry)
        {
            var entryElement = towerEntryTemplate.Instantiate();

            var nameLabel = entryElement.Q<Label>("DefenseName");
            var thumbnail = entryElement.Q<Image>("Thumbnail");
            var buildButton = entryElement.Q<Button>("BuildButton");

            nameLabel.text = entry.DefenseEntryName;
            if (entry.Thumbnail != null)
            {
                thumbnail.sprite = entry.Thumbnail;
            }
            else
            {
                thumbnail.image = null;
            }

            buildButton.clickable.clicked += () =>
            {
                if (_currencyStorage.Spend(CurrencyType.Sunlight, entry.Cost))
                {
                    _buildManager.SelectPrefab(entry.DefensePrefab);
                }
                else
                {
                    Debug.LogWarning($"Not enough sunlight to build {entry.DefensePrefab.name}");
                }
            };

            panel.Add(entryElement);
            _defenseEntries.Add(entry);
        }
    }
}
