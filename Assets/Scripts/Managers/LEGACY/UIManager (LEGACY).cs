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


// public class UIManager : MonoBehaviour
// {
//     [Header("UI Toolkit")]
//     [SerializeField] UIDocument _sidePanelDocument;
//     [SerializeField] UIDocument _HUDDocument;
//     [SerializeField] VisualTreeAsset _towerEntryTemplate;

//     [Header("Tower Data")]
//     [SerializeField] TowerConfig _towerDefenses;

//     [Header("Currency UI")]
//     Label _sunLabel;
//     Label _waterLabel;

//     [Header("Fields")]
//     VisualElement _sidePanelRoot;
//     VisualElement _HUDRoot;
//     VisualElement panel;
//     Button collapseButton;
//     ListView _towerList;

//     List<DefenseEntry> _defenseEntries = new();

//     CurrencyStorage _currencyStorage;
//     BuildManager _buildManager;

//     void Awake()
//     {
//         // Find game object references
//         _currencyStorage = FindFirstObjectByType<CurrencyStorage>();
//         _buildManager = FindFirstObjectByType<BuildManager>();
//     }

//     void Start()
//     {
//         // Cache UI elements from the _sidePanelDocument
//         _sidePanelRoot = _sidePanelDocument.rootVisualElement;
//         panel = _sidePanelRoot.Q<VisualElement>("Panel");
//         collapseButton = _sidePanelRoot.Q<Button>("CollapseButton");
//         _towerList = _sidePanelRoot.Q<ListView>("TowerList");

//         // Currency labels (make sure they're in root, not in panel)
//         _HUDRoot = _HUDDocument.rootVisualElement;
//         _sunLabel = _HUDRoot.Q<Label>("SunLabel");
//         _waterLabel = _HUDRoot.Q<Label>("WaterLabel");

//         // Set up collapse button
//         collapseButton.RegisterCallback<ClickEvent>(OnCollapseClicked);
//         SetupCollapseButtonPosition(isCollapsed: true);

//         // Hide panel on start
//         panel.style.display = DisplayStyle.None;

//         // Set up the tower UI
//         PopulateDefenseEntries();
//         SetupListView();
//     }


//     #region SUBSCRIPTIONS
//     void OnEnable()
//     {
//         GameEventsManager.OnCurrencyUpdate += UpdateCurrencyUI;
//         // GameEventsManager.OnLevelUpUpdate += UpdateLevelUI;
//         // GameEventsManager.OnExpGainUpdate += UpdateExpText;
//     }

//     void OnDisable()
//     {
//         GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
//         // GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
//         // GameEventsManager.OnExpGainUpdate -= UpdateExpText;
//     }
//     #endregion

//     #region EVENT SUBSCRIBERS
//     void UpdateCurrencyUI(CurrencyType type, float value)
//     {
//         string formatted = $"{type}: {NumberFormatter.Format(value)}";

//         switch (type)
//         {
//             case CurrencyType.Water:
//                 _waterLabel.text = formatted;
//                 break;
//             case CurrencyType.Sunlight:
//                 _sunLabel.text = formatted;
//                 break;
//             default:
//                 Debug.LogWarning($"Unhandled currency type: {type}");
//                 break;
//         }
//     }
//     #endregion

//     void OnCollapseClicked(ClickEvent evt)
//     {
//         bool isNowOpen = panel.style.display == DisplayStyle.None;
//         panel.style.display = isNowOpen ? DisplayStyle.Flex : DisplayStyle.None;
//         SetupCollapseButtonPosition(!isNowOpen);

//         Debug.Log(isNowOpen ? "Open" : "Close");
//     }

//     void SetupCollapseButtonPosition(bool isCollapsed)
//     {
//         collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
//         collapseButton.style.left = new StyleLength(new Length(isCollapsed ? 2 : 18.5f, LengthUnit.Percent));
//     }

//     void PopulateDefenseEntries()
//     {
//         foreach (var defense in _towerDefenses.DefenseEntry)
//         {
//             defense.UpgradeLogic = () =>
//             {
//                 if (_currencyStorage.Spend(CurrencyType.Sunlight, defense.Cost))
//                 {
//                     _buildManager.SelectPrefab(defense.DefensePrefab);
//                 }
//                 else
//                 {
//                     Debug.LogWarning($"Not enough sunlight to build {defense.DefensePrefab.name}");
//                 }
//             };

//             _defenseEntries.Add(defense);
//         }
//     }

//     void SetupListView()
//     {
//         _towerList.makeItem = () => _towerEntryTemplate.Instantiate();
//         _towerList.bindItem = (element, index) =>
//         {
//             var data = _defenseEntries[index];
//             var nameLabel = element.Q<Label>("DefenseName");
//             var thumbnail = element.Q<Image>("Thumbnail");
//             var buildButton = element.Q<Button>("BuildButton");
//             var damageLabel = element.Q<Label>("DMG");
//             var rangeLabel = element.Q<Label>("RNG");
//             var speedLabel = element.Q<Label>("SPD");

//             // Transform triangleTransform = data.DefensePrefab.transform.Find("Sprites/Triangle");

//             TowerStatConfig towerData = data.DefensePrefab.GetComponent<TowerDefense>().StatConfig;

//             // Tower Entry
//             if (data.Thumbnail != null)
//             {
//                 // For when have actual tower sprites
//                 // SpriteRenderer spriteRenderer = triangleTransform.GetComponent<SpriteRenderer>();
//                 // thumbnail.sprite = spriteRenderer.sprite;

//                 thumbnail.sprite = data.Thumbnail;
//             }
//             else
//             {
//                 thumbnail.image = null;
//             }
//             nameLabel.text = data.DefenseEntryName;

//             damageLabel.text = "DMG: " + towerData.GetValue(TowerStats.Attack);
//             rangeLabel.text = "RNG: " + towerData.GetValue(TowerStats.Range);
//             speedLabel.text = "SPD: " + towerData.GetValue(TowerStats.Speed);

//             buildButton.text = "Cost " + data.Cost.ToString();
//             buildButton.clickable.clicked += data.UpgradeLogic;
//         };

//         _towerList.itemsSource = _defenseEntries;
//         _towerList.selectionType = SelectionType.None;
//         _towerList.fixedItemHeight = 100;
//     }
// }
