using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UpgradeUI : MonoBehaviour
{
    TextMeshProUGUI _upgradeNameText;
    TextMeshProUGUI _rateText;
    TextMeshProUGUI _buttonLabelText;
    TextMeshProUGUI _levelText;
    Button _upgradeButton;

    public void Setup(string upgradeName,  UnityAction onClick)
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

            _upgradeButton = GetComponentInChildren<Button>();
            _buttonLabelText = _upgradeButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        _upgradeNameText.text = upgradeName;
        _upgradeButton.onClick.RemoveAllListeners();
        _upgradeButton.onClick.AddListener(onClick);
    }

    public void Refresh(string rateText, string levelText, string buttonText)
    {
        _rateText.text = rateText;
        _levelText.text = levelText;
        _buttonLabelText.text = buttonText;
    }
}
