using UnityEngine;
using UnityEngine.UIElements;

public class HUDController : MonoBehaviour
{
    [SerializeField] UIDocument _hudDocument;

    Label _sunLabel;
    Label _waterLabel;

    void Start()
    {
        var root = _hudDocument.rootVisualElement;
        _sunLabel = root.Q<Label>("SunLabel");
        _waterLabel = root.Q<Label>("WaterLabel");
    }

    void OnEnable() => GameEventsManager.OnCurrencyUpdate += UpdateUI;
    void OnDisable() => GameEventsManager.OnCurrencyUpdate -= UpdateUI;

    void UpdateUI(CurrencyType type, float value)
    {
        string formatted = $"{type}: {NumberFormatter.Format(value)}";
        if (type == CurrencyType.Sunlight) _sunLabel.text = formatted;
        else if (type == CurrencyType.Water) _waterLabel.text = formatted;
    }
}
