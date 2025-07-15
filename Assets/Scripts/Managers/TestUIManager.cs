using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUIManager : MonoBehaviour
{
    [SerializeField] private UIDocument uIDocument;
    private VisualElement root;

    private VisualElement panel;

    private VisualElement collapseButton;
    void Start()
    {
        root = uIDocument.rootVisualElement;
        collapseButton = root.Q<VisualElement>("CollapseButton");
        panel = root.Q<VisualElement>("Panel");

        panel.style.display = DisplayStyle.None;
        collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
        collapseButton.style.left = new StyleLength(new Length(2, LengthUnit.Percent)); 

        // Register the callback
        collapseButton.RegisterCallback<ClickEvent>(OnClickEvent);
    }

    private void OnClickEvent(ClickEvent evt)
    {
        panel.style.display = panel.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
        if (panel.style.display == DisplayStyle.Flex)
        {
            collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));
            collapseButton.style.left = new StyleLength(new Length(18.5f, LengthUnit.Percent));
        }
        else
        {
            collapseButton.style.top = new StyleLength(new Length(4, LengthUnit.Percent));  
            collapseButton.style.left = new StyleLength(new Length(2, LengthUnit.Percent)); 
        }
    }
}
