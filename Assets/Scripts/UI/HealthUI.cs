using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour, IHealthUI
{
    [SerializeField] Image _bar;
    GameObject _container;

    void Awake()
    {
        if (_bar != null && _bar.transform.parent != null)
            _container = _bar.transform.parent.gameObject;
        else
            _container = _bar != null ? _bar.gameObject : null; // fallback
    }

    public void Show()
    {
        if (_container != null && !_container.activeSelf)
            _container.SetActive(true);
    }

    public void UpdateBar(float current, float max)
    {
        if (_bar != null)
            _bar.fillAmount = current / max;
    }
}
