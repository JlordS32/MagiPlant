using System;
using UnityEngine;

public abstract class IconLookupBase : ScriptableObject
{
    public abstract Sprite GetIcon(Enum enumKey);
    public abstract Type GetEnumType();
}
