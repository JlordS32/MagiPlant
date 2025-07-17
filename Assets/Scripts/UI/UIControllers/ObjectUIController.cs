using UnityEngine;
using UnityEngine.UIElements;

public class ObjectUIController : MonoBehaviour
{
    UIDocument _uiDocument;
    VisualElement _panel;

    void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing on the ObjectUIController GameObject.");
            return;
        }

        _panel = _uiDocument.rootVisualElement.Q<VisualElement>("Panel");
        if (_panel == null)
        {
            Debug.LogError("Panel element not found in the UIDocument.");
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
