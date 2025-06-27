using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] AudioClip _bgMusic;

    void Start()
    {
        if (_bgMusic)
            AudioManager.Instance.PlayMusic(_bgMusic);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}