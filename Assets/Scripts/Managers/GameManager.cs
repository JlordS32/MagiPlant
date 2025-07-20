using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // PROPERTIES
    [SerializeField] InputAction _pauseButton;
    [SerializeField] GameObject _pausePanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debugger.LogWarning(DebugCategory.Singletons, "Multiple instances of GameManager detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        _pauseButton.Enable();
    }

    void OnDisable()
    {
        _pauseButton.Disable();
    }

    void Update()
    {
        if (_pauseButton.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    // TODO[DUY]: Switch pause to using UI Toolkit Document
    public void TogglePause()
    {
        AudioManager.Instance.PauseMusic();
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        _pausePanel.SetActive(!_pausePanel.activeSelf);
    }
}