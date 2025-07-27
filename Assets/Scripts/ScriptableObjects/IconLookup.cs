using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Icon Lookup")]
public class IconLookup : ScriptableObject
{
    [Serializable]
    public struct IconEntry
    {
        public string Name;
        public CurrencyType Type;
        public Sprite Icon;
    }

    public IconEntry[] entries;

    private Dictionary<CurrencyType, Sprite> _lookup;

    public Sprite GetIcon(CurrencyType type)
    {
        if (_lookup == null)
        {
            _lookup = new Dictionary<CurrencyType, Sprite>();
            foreach (var entry in entries)
                _lookup[entry.Type] = entry.Icon;
        }

        return _lookup.TryGetValue(type, out var icon) ? icon : null;
    }
}
