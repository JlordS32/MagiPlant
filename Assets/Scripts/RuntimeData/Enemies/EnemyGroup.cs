using UnityEngine;
using System;

[Serializable]
public class EnemyGroup
{
    public GameObject Prefab;
    public float Gap = 1f;
    public int Count = 5;
}

[Serializable]
public class Wave
{
    public EnemyGroup[] Groups;
}