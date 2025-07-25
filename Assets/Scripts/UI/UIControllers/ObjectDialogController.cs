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
        _statContainer = root.Q<VisualElement>("StatContainer");
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
    }

    // WARNING: Unused param
    // TODO: Show the stats on the GUI
    public void Show(PlacedObject placedObject)
    {
        _data = placedObject.GetData();

        foreach (var stat in _data.GetStats())
        {
            float current = stat.Value;
            float max = _data.GetMaxLevelValue(stat.Key);
            float baseVal = _data.GetBaseValue(stat.Key);

            Label statLabel = new($"{stat.Key} : {current} / {max} (base: {baseVal})");
            _statContainer.Add(statLabel);
        }

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
