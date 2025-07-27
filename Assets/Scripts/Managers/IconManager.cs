using UnityEngine;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }
    [SerializeField] private IconLookup _currencyIconLookup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Sprite GetCurrencyIcon(CurrencyType type)
    {
        if (_currencyIconLookup == null)
        {
            Debugger.LogError(DebugCategory.UI, "Currency Icon Look up is not implemented!");
        }
        return _currencyIconLookup.GetIcon(type);
    }
}
