using UnityEngine;

public class ObjectClicker : MonoBehaviour
{
    [SerializeField] CurrencyType _currencyType;
    [SerializeField] AudioClip _clickSound;

    CurrencyClicker _currencyClick;

    void Awake()
    {
        _currencyClick = FindFirstObjectByType<CurrencyClicker>();
    }

    void OnMouseDown()
    {
        AudioManager.Instance.PlaySound(_clickSound);
        CropIsPressed();
    }

    public void CropIsPressed()
    {
        if (_currencyClick != null)
        {
            _currencyClick.Click(_currencyType);
        }
    }
}
