using System;
using UnityEngine;

[Serializable]
public class DefenseEntry
{
    public GameObject DefensePrefab;
    public Sprite Thumbnail;
    public string Name;
    public float Cost;

    [NonSerialized] public Action UpgradeLogic;
}