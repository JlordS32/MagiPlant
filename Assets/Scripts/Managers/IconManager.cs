using UnityEngine;
using System;
using System.Collections.Generic;

public class IconManager : MonoBehaviour
{
    public static IconManager Instance { get; private set; }

    [Serializable]
    public struct LookupEntry
    {
        public string Name;
        public IconLookupBase Lookup;
    }

    [SerializeField] private LookupEntry[] _entries;
    private Dictionary<Type, IconLookupBase> _lookupMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _lookupMap = new();
        foreach (var entry in _entries)
        {
            var baseType = entry.Lookup.GetEnumType();
            if (!_lookupMap.ContainsKey(baseType))
                _lookupMap[baseType] = entry.Lookup;
        }
    }

    public Sprite GetIcon(Enum enumKey)
    {
        var type = enumKey.GetType();

        if (_lookupMap != null && _lookupMap.TryGetValue(type, out var lookup))
            return lookup.GetIcon(enumKey);

        Debug.LogError($"No icon lookup found for enum type: {type}");
        return null;
    }
}
