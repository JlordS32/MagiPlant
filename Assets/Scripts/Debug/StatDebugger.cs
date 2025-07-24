using UnityEngine;
using System;
using System.Collections.Generic;

public enum ConfigType
{
    Player,
    Enemy,
    Resource,
    Tower
}

[Serializable]
public struct StatConfigEntry
{
    public string Name;
    public ConfigType Type;
    ScriptableObject StatConfig;
}

public class StatDebugger : MonoBehaviour
{
    [SerializeField] StatConfigEntry[] _statConfigs;

    Dictionary<ConfigType, StatConfigEntry> _statLookUp;



    [ContextMenu("Print All Config Progressions")]
    void PrintAll()
    {
        foreach (var config in _statConfigs)
        {
            // if (config is )
        }
    }
}