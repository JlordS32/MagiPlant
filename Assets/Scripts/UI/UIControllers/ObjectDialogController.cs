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
        _closeBtn = root.Q<Button>("CloseButton");

        // Setup
        _closeBtn.RegisterCallback<ClickEvent>(Hide);
        _modal.AddToClassList("hidden");
    }

    public void Hide(ClickEvent evt)
    {
        ToggleModal(false);

        // Enable Object UI Controller back
        ObjectUIController.Instance.ToggleObjectUI(true);
    }

    public void Show(VisualTreeAsset content, PlacedObject placedObject)
    {
        if (content == null || placedObject == null)
        {
            Debugger.LogError(DebugCategory.UI, "Missing references on either content or place objects!");
        }

        // SETTING UP new #Content below
        // Fetch references first
        VisualElement contentDoc = content.CloneTree();
        VisualElement contents = contentDoc.Q<VisualElement>("Content");
        VisualElement statContainer = contents.Q<VisualElement>("StatContainer");
        VisualElement descContainer = contents.Q<VisualElement>("Description");
        Label title = contents.Q<Label>("Title");

        // Get data
        _data = placedObject.GetData();

        // Set title
        title.text = $"{placedObject.Entry.BuildEntryName} (Level {_data.Level})";

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

            statContainer.Add(progressBarElement);
        }

        TextElement desc = new()
        {
            text = _data.GetDescription() ?? ""
        };

        descContainer.Add(desc);

        // Replace #Content
        VisualElement oldContent = _modal.Q<VisualElement>("Content");
        var parent = oldContent.parent;
        var index = parent.IndexOf(oldContent);
        parent.RemoveAt(index);
        parent.Insert(index, contents); // Insert the new #Content

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
