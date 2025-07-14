#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PhaseListenerWindow : EditorWindow
{
    [MenuItem("Window/Debug/Phase Listeners")]
    static void Open() => GetWindow<PhaseListenerWindow>("Phase Listeners");

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play‑mode to see listeners.", MessageType.Info);
            return;
        }

        var listeners = PhaseService.Listeners;
        EditorGUILayout.LabelField($"Current Phase: {PhaseService.Current}");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Listeners ({listeners.Count})", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        foreach (var l in listeners)
            EditorGUILayout.LabelField("• " + l.GetType().Name);
    }
}
#endif
