using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    [SerializeField] UIDocument _hudDocument;

    Label _sunLabel;
    Label _waterLabel;
    Label _timeLabel;

    void Start()
    {
        var root = _hudDocument.rootVisualElement;
        _sunLabel = root.Q<Label>("SunLabel");
        _waterLabel = root.Q<Label>("WaterLabel");
        _timeLabel = root.Q<Label>("TimeLabel");
    }

    void Update()
    {
        _timeLabel.text = $"{TimeManager.Instance.GetTimeString()}";
    }

    void OnEnable() => GameEventsManager.OnCurrencyUpdate += UpdateUI;
    void OnDisable() => GameEventsManager.OnCurrencyUpdate -= UpdateUI;

    void UpdateUI(CurrencyType type, float value)
    {
        string formatted = $"{NumberFormatter.Format(value)}";
        if (type == CurrencyType.Sunlight) _sunLabel.text = formatted;
        else if (type == CurrencyType.Water) _waterLabel.text = formatted;
    }
}
