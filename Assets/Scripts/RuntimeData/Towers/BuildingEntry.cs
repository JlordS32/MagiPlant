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

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Prefab && Prefab.GetComponent<IBuildable>() == null)
        {
            Debug.LogWarning($"{Prefab.name} isn’t a defence prefab", Prefab);
            Prefab = null;      // optional: auto-clear
        }
    }
#endif
}