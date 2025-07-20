using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CategoryEntry
{
    public string Name;
    public DebugCategory Category;
    public bool IsEnabled = true;
}

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }

    [SerializeField] List<CategoryEntry> _categories = new();
    Dictionary<DebugCategory, bool> _categoryMap;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeCategoryMap();
    }

    void InitializeCategoryMap()
    {
        _categoryMap = new();
        foreach (var entry in _categories)
            _categoryMap[entry.Category] = entry.IsEnabled;
    }

#if UNITY_EDITOR
    public void Log(DebugCategory category, string message)
    {
        if (_categoryMap.TryGetValue(category, out bool enabled) && enabled)
            Debug.Log($"[{category}] {message}");
    }

    public void LogWarning(DebugCategory category, string message)
    {
        if (_categoryMap.TryGetValue(category, out bool enabled) && enabled)
            Debug.LogWarning($"[{category}] {message}");
    }

    public void LogError(DebugCategory category, string message)
    {
        if (_categoryMap.TryGetValue(category, out bool enabled) && enabled)
            Debug.LogError($"[{category}] {message}");
    }
#endif
}

public static class Debugger
{
#if UNITY_EDITOR
    public static void Log(DebugCategory category, string message)
    {
        if (DebugManager.Instance != null)
            DebugManager.Instance.Log(category, message);
    }

    public static void LogWarning(DebugCategory category, string message)
    {
        if (DebugManager.Instance != null)
            DebugManager.Instance.LogWarning(category, message);
    }

    public static void LogError(DebugCategory category, string message)
    {
        if (DebugManager.Instance != null)
            DebugManager.Instance.LogError(category, message);
    }
#else
    public static void Log(DebugCategory category, string message) { }
    public static void LogWarning(DebugCategory category, string message) { }
    public static void LogError(DebugCategory category, string message) { }
#endif
}
