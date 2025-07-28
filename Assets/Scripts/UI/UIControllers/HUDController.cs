using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    [SerializeField] UIDocument _hudDocument;

    Label _sunLabel;
    Label _waterLabel;
    Label _timeLabel;
    Label _expLabel;
    Label _levelLabel;
    VisualElement _sunCurrencyUI;
    VisualElement _waterCurrencyUI;

    void Start()
    {
        var root = _hudDocument.rootVisualElement;
        var hud = root.Q<VisualElement>("HUD");
        if (hud == null) { Debug.LogError("HUD not found"); return; }

        _sunCurrencyUI = hud.Q<VisualElement>("SunCurrency");
        _waterCurrencyUI = hud.Q<VisualElement>("WaterCurrency");

        _sunLabel = hud.Q<Label>("SunLabel");
        _waterLabel = hud.Q<Label>("WaterLabel");
        _timeLabel = hud.Q<Label>("TimeLabel");
        _expLabel = hud.Q<Label>("ExpLabel");
        _levelLabel = hud.Q<Label>("LevelLabel");

        var sunIcon = _sunCurrencyUI.Q<VisualElement>("Icon");
        sunIcon.style.backgroundImage = new StyleBackground(IconManager.Instance.GetIcon(CurrencyType.Sunlight));
        var waterIcon = _waterCurrencyUI.Q<VisualElement>("Icon");
        waterIcon.style.backgroundImage = new StyleBackground(IconManager.Instance.GetIcon(CurrencyType.Water));
    }

    void Update()
    {
        _timeLabel.text = $"{TimeManager.Instance.GetTimeString()}";
    }

    void OnEnable()
    {
        GameEventsManager.OnCurrencyUpdate += UpdateCurrencyUI;
        GameEventsManager.OnExpGainUpdate += UpdateExpUI;
        GameEventsManager.OnLevelUpUpdate += UpdateLevelUI;
    }

    void OnDisable()
    {
        GameEventsManager.OnCurrencyUpdate -= UpdateCurrencyUI;
        GameEventsManager.OnExpGainUpdate -= UpdateExpUI;
        GameEventsManager.OnLevelUpUpdate -= UpdateLevelUI;
    }

    void UpdateCurrencyUI(CurrencyType type, float value)
    {
        string formatted = NumberFormatter.Format(value);
        if (type == CurrencyType.Sunlight) _sunLabel.text = formatted;
        else if (type == CurrencyType.Water) _waterLabel.text = formatted;
    }

    void UpdateExpUI(float currentExp, float requiredExp)
    {
        _expLabel.text = $"EXP: {NumberFormatter.Format(currentExp)} / {NumberFormatter.Format(requiredExp)}";
    }

    void UpdateLevelUI(int value)
    {
        _levelLabel.text = $"Level: {NumberFormatter.Format(value)}";
    }
}
