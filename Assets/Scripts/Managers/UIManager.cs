using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debugger.LogWarning(DebugCategory.Singletons, "Multiple instances of UIManager detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Add UI management methods here
}
