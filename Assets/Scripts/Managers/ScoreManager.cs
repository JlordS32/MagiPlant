using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debugger.LogWarning(DebugCategory.Singletons, "Multiple instances of ScoreManager detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    int _score;

    public int Score => _score;
    public void IncreaseScore(int value)
    {
        _score += value;
    }
}