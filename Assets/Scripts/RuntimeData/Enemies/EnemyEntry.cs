using UnityEngine;
using System;

[Serializable]
public class EnemyEntry
{
    public string EnemyEntryName;
    public GameObject EnemyPrefab;
    public EnemyType Type;
    public EnemyTrait Traits;
    public int Cost = 1;
}