using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class UpgradeUI : MonoBehaviour
{
    TextMeshProUGUI _upgradeNameText;
    TextMeshProUGUI _rateText;
    TextMeshProUGUI _buttonLabelText;
    TextMeshProUGUI _levelText;
    Button _upgradeButton;

    Func<string> _getRate;
    Func<string> _getButtonLabel;
    Func<string> _getLevel;

    public void Setup(string upgradeName, Func<string> getRate, Func<string> getButtonLabel, Func<string> getLevel, UnityAction onClick)
    {
        if (_upgradeNameText == null || _rateText == null || _upgradeButton == null)
        {
            var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var text in texts)
            {
                if (text.name == "Upgrade Name") _upgradeNameText = text;
                else if (text.name == "Level") _levelText = text;
                else if (text.name == "Rate") _rateText = text;
            }

            _upgradeButton = GetComponentInChildren<Button>(true);
            _buttonLabelText = _upgradeButton.GetComponentInChildren<TextMeshProUGUI>(true); // this line matters
        }

        _upgradeNameText.text = upgradeName;
        _getRate = getRate;
        _getButtonLabel = getButtonLabel;
        _getLevel = getLevel;

        _rateText.text = _getRate?.Invoke();
        _buttonLabelText.text = _getButtonLabel?.Invoke();
        _levelText.text = _getLevel?.Invoke();

        _upgradeButton.onClick.RemoveAllListeners();
        _upgradeButton.onClick.AddListener(onClick);
    }

    public void Update()
    {
        if (_getRate != null && _rateText != null)
            _rateText.text = _getRate.Invoke();

        if (_getButtonLabel != null && _buttonLabelText != null)
            _buttonLabelText.text = _getButtonLabel.Invoke();

        if (_getLevel != null && _levelText != null)
            _levelText.text = _getLevel.Invoke();
    }
}
