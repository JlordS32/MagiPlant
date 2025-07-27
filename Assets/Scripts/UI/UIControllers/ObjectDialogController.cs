using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectDialogController : MonoBehaviour
{
    public static ObjectDialogController Instance { get; private set; }

    [SerializeField] UIDocument _dialogDocument;
    [SerializeField] VisualTreeAsset _progressBar;

    VisualElement _modal;
    VisualElement _statContainer;
    VisualElement _descriptionContainer;
    Label _title;
    Button _closeBtn;
    Camera _cam;
    IStatData _data;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debugger.LogWarning(DebugCategory.Singletons, "Multiple instances of ObjectDialogController detected. Destroying the new instance.");

            Destroy(gameObject);
            return;
        }

        Instance = this;
        _cam = Camera.main;
    }

    void Start()
    {
        var root = _dialogDocument.rootVisualElement;
        _modal = root.Q<VisualElement>("Modal");
        _title = root.Q<Label>("Title");
        _statContainer = root.Q<VisualElement>("StatContainer");
        _descriptionContainer = root.Q<VisualElement>("Description");
        _closeBtn = root.Q<Button>("CloseButton");

        _closeBtn.RegisterCallback<ClickEvent>(Hide);
        _modal.AddToClassList("hidden");
    }

    public void Hide(ClickEvent evt)
    {
        ToggleModal(false);

        // Enable Object UI Controller back
        ObjectUIController.Instance.ToggleObjectUI(true);
        _statContainer.Clear();
        _descriptionContainer.Clear();
    }

    // WARNING: Unused param
    // TODO: Show the stats on the GUI
    public void Show(PlacedObject placedObject)
    {
        _data = placedObject.GetData();

        // Set title
        _title.text = $"{placedObject.Entry.BuildEntryName} (Level {_data.Level})";

        foreach (var stat in _data.GetStats())
        {
            VisualElement progressBarElement = _progressBar.CloneTree();
            var bar = progressBarElement.Q<ProgressBar>("ProgressBar");
            var label = progressBarElement.Q<Label>("Name");

            label.text = $"{stat.Key}: ";
            bar.title = $"{stat.Value}";
            bar.value = stat.Value;
            bar.lowValue = _data.GetBaseValue(stat.Key);
            bar.highValue = _data.GetMaxLevelValue(stat.Key);

            _statContainer.Add(progressBarElement);
        }

        TextElement desc = new()
        {
            text = _data.GetDescription() ?? ""
        };

        _descriptionContainer.Add(desc);

        ToggleModal(true);
    }

    public void ToggleModal(bool toggle)
    {
        _modal.EnableInClassList("active", toggle);
        _modal.EnableInClassList("hidden", !toggle);

        // DISABLE CAMERA MOVEMENTS
        _cam.GetComponent<CameraDrag>().enabled = !toggle;
        _cam.GetComponent<CameraZoom>().enabled = !toggle;
    }
}
