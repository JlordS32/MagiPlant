using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _defenseNameText;
    [SerializeField] Button _buttonAction;
    [SerializeField] Image _thumbnail;

    public void Setup(string upgradeName, Sprite thumbnail, UnityAction onClick)
    {
        _defenseNameText.text = upgradeName;
        _thumbnail.sprite     = thumbnail;

        _buttonAction.onClick.RemoveAllListeners();
        _buttonAction.onClick.AddListener(onClick);
    }
}
