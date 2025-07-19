using UnityEngine;
using UnityEngine.UIElements;

public class ObjectDialogController : MonoBehaviour
{
    public static ObjectDialogController Instance { get; private set; }

    [SerializeField] UIDocument _dialogDocument;
    [SerializeField] VisualTreeAsset _progressBar;

    VisualElement _modal;
    Button _closeBtn;
    Camera _cam;

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
        _closeBtn = root.Q<Button>("CloseButton");

        _closeBtn.RegisterCallback<ClickEvent>(Hide);
        _modal.AddToClassList("hidden");
    }

    public void Hide(ClickEvent evt)
    {
        _modal.AddToClassList("hidden");
        _modal.RemoveFromClassList("active");

        // Disable some camera controls
        _cam.GetComponent<CameraDrag>().enabled = true;
        _cam.GetComponent<CameraZoom>().enabled = true;

        // Enable Object UI Controller back
        ObjectUIController.Instance.ToggleObjectUI(true);
    }

    public void Show(PlacedObject placedObject)
    {
        _modal.AddToClassList("active");
        _modal.RemoveFromClassList("hidden");

        // Disable some camera controls
        _cam.GetComponent<CameraDrag>().enabled = false;
        _cam.GetComponent<CameraZoom>().enabled = false;
    }
}
