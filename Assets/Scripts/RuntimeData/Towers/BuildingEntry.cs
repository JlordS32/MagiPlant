using System;
using UnityEngine;

[Serializable]
public class BuildingEntry
{
    public string BuildEntryName;
    public BuildingType Category;
    public GameObject Prefab;
    public Sprite Thumbnail;
    public float Cost;

    [NonSerialized] public Action BuildingLogic;
}