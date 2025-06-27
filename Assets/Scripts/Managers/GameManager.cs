using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    // PROPERTIES
    [SerializeField] InputAction _pauseButton;
    [SerializeField] GameObject _pausePanel;

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
        if (_pauseButton.WasPressedThisFrame()) // !_isDead
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        AudioManager.Instance.PauseMusic();
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        _pausePanel.SetActive(!_pausePanel.activeSelf);
    }
}