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

    [Header("Currency UI")]
    private Label sunLabel;
    private Label waterLabel;

    [Header("Private Fields")]
    private VisualElement root;
    private VisualElement panel;
    private Button collapseButton;
    private ListView towerList;

    private List<DefenseEntry> _defenseEntries = new();

    private CurrencyStorage _currencyStorage;
    private BuildManager _buildManager;

    void Awake()
    {
        // Find game object references
        _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
        _buildManager = FindFirstObjectByType<BuildManager>();
    }

    void Start()
    {
        // Cache UI elements from the UIDocument
        root = uiDocument.rootVisualElement;
        panel = root.Q<VisualElement>("Panel");
        collapseButton = root.Q<Button>("CollapseButton");
        towerList = root.Q<ListView>("TowerList");

        // Currency labels (make sure they're in root, not in panel)
        sunLabel = root.Q<Label>("SunLabel");
        waterLabel = root.Q<Label>("WaterLabel");

        // Set up collapse button
        collapseButton.RegisterCallback<ClickEvent>(OnCollapseClicked);
        SetupCollapseButtonPosition(isCollapsed: true);

        // Hide panel on start
        panel.style.display = DisplayStyle.None;

        // Set up the tower UI
        PopulateDefenseEntries();
        SetupListView();
    }


    #region SUBSCRIPTIONS
    void OnEnable()
    {
        GameEventsManager.OnCurrencyUpdate += UpdateCurrencyUI;
        // GameEventsManager.OnLevelUpUpdate += UpdateLevelUI;
        // GameEventsManager.OnExpGainUpdate += UpdateExpText;
    }

    void OnDisable()
    {
        GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
        // GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
        // GameEventsManager.OnExpGainUpdate -= UpdateExpText;
    }
    #endregion

    #region EVENT SUBSCRIBERS
    void UpdateCurrencyUI(CurrencyType type, float value)
    {
        string formatted = $"{type}: {NumberFormatter.Format(value)}";

        switch (type)
        {
            case CurrencyType.Water:
                waterLabel.text = formatted;
                break;
            case CurrencyType.Sunlight:
                sunLabel.text = formatted;
                break;
            default:
                Debug.LogWarning($"Unhandled currency type: {type}");
                break;
        }
    }
    #endregion

    void OnCollapseClicked(ClickEvent evt)
    {
        bool isNowOpen = panel.style.display == DisplayStyle.None;
        panel.style.display = isNowOpen ? DisplayStyle.Flex : DisplayStyle.None;
        SetupCollapseButtonPosition(!isNowOpen);

        Debug.Log(isNowOpen ? "Open" : "Close");
    }

    private void SetupCollapseButtonPosition(bool isCollapsed)
    {
        collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
        collapseButton.style.left = new StyleLength(new Length(isCollapsed ? 2 : 18.5f, LengthUnit.Percent));
    }

    private void PopulateDefenseEntries()
    {
        foreach (var defense in _towerDefenses.DefenseEntry)
        {
            defense.UpgradeLogic = () =>
            {
                if (_currencyStorage.Spend(CurrencyType.Sunlight, defense.Cost))
                {
                    _buildManager.SelectPrefab(defense.DefensePrefab);
                }
                else
                {
                    Debug.LogWarning($"Not enough sunlight to build {defense.DefensePrefab.name}");
                }
            };

            _defenseEntries.Add(defense);
        }
    }

    private void SetupListView()
    {
        towerList.makeItem = () => towerEntryTemplate.Instantiate();
        towerList.bindItem = (element, index) =>
        {
            var data = _defenseEntries[index];
            var nameLabel = element.Q<Label>("DefenseName");
            var thumbnail = element.Q<Image>("Thumbnail");
            var buildButton = element.Q<Button>("BuildButton");
            var damageLabel = element.Q<Label>("DMG");
            var rangeLabel = element.Q<Label>("RNG");
            var speedLabel = element.Q<Label>("SPD");

            // Transform triangleTransform = data.DefensePrefab.transform.Find("Sprites/Triangle");

            var towerStats = data.DefensePrefab.GetComponent<TowerDefense>();
            TowerStatConfig towerData = towerStats.GetTowerStats();
            
            // Tower Entry
            if (data.Thumbnail != null)
            {
                // For when have actual tower sprites
                // SpriteRenderer spriteRenderer = triangleTransform.GetComponent<SpriteRenderer>();
                // thumbnail.sprite = spriteRenderer.sprite;

                thumbnail.sprite = data.Thumbnail;
            }
            else
            {
                thumbnail.image = null;
            }
            nameLabel.text = data.DefenseEntryName;
            damageLabel.text = "DMG: " + towerData.Attack.ToString();
            rangeLabel.text = "RNG: " + towerData.Range.ToString();
            speedLabel.text = "SPD: " + towerData.Speed.ToString();


            buildButton.text = "Cost " + data.Cost.ToString();
            buildButton.clickable.clicked +=  data.UpgradeLogic;
        };

        towerList.itemsSource = _defenseEntries;
        towerList.selectionType = SelectionType.None;
        towerList.fixedItemHeight = 100; 
    }
}
