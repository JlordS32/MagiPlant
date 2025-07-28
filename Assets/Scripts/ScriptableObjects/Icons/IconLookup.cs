using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class IconLookup<TEnum> : IconLookupBase where TEnum : Enum
{
    [Serializable]
    public struct IconEntry
    {
        public string Name;
        public TEnum Type;
        public Sprite Icon;
    }

    [SerializeField] private IconEntry[] entries;
    Dictionary<TEnum, Sprite> _lookup;

    public override Sprite GetIcon(Enum type)
    {
        if (type is TEnum key)
            return GetIconTyped(key);

        Debugger.LogError(DebugCategory.Services, "Attempt to fetch icon but icon entry does not exist!");
        return null;
    }

    public Sprite GetIconTyped(TEnum type)
    {
        if (_lookup == null)
        {
            _lookup = new();
            foreach (var iconEntry in entries)
                _lookup[iconEntry.Type] = iconEntry.Icon;
        }

        return _lookup.TryGetValue(type, out var icon) ? icon : null;
    }

    public override Type GetEnumType() => typeof(TEnum);
}
