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

    void Start()
    {
        var root = _hudDocument.rootVisualElement;
        _sunLabel = root.Q<Label>("SunLabel");
        _waterLabel = root.Q<Label>("WaterLabel");
        _timeLabel = root.Q<Label>("TimeLabel");
        _expLabel = root.Q<Label>("ExpLabel");
        _levelLabel = root.Q<Label>("LevelLabel");
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
