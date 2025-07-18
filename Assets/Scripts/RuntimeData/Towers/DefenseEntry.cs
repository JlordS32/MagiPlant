using System;
using UnityEngine;

[Serializable]
public class DefenseEntry
{
    public string DefenseEntryName;
    public GameObject DefensePrefab;
    public Sprite Thumbnail;
    public float Cost;

    [NonSerialized] public Action UpgradeLogic;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (DefensePrefab &&
            DefensePrefab.GetComponent<TowerDefense>() == null)
        { 
            Debug.LogWarning($"{DefensePrefab.name} isn’t a defence prefab", DefensePrefab);
            DefensePrefab = null;      // optional: auto-clear
        }
    }
#endif
}