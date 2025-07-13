using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class DefenseEntry
{
    public GameObject DefensePrefab;
    public Sprite Thumbnail;
    public string Name;
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