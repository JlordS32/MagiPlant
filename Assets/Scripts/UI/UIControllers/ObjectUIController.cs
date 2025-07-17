using UnityEngine;
using UnityEngine.UIElements;

public class ObjectUIController : MonoBehaviour
{
    public static ObjectUIController Instance { get; private set; }

    [SerializeField] UIDocument _uiDocument;
    VisualElement _panel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of ObjectUIController detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _panel = _uiDocument.rootVisualElement.Q<VisualElement>("Panel");
        if (_panel == null)
        {
            Debug.LogError("Panel element not found in the UIDocument.");
            return;
        }

        _panel.style.display = DisplayStyle.None;
    }

    public void Show()
    {
        _panel.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        _panel.style.display = DisplayStyle.None;
    }

}
