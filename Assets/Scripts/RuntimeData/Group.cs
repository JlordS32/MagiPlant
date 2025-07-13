using UnityEngine;
using System;

[Serializable]
public class Group
{
    public GameObject prefab;
    public float gap = 1f;
    public int count = 5;
}

[Serializable]
public class Wave
{
    public Group[] groups;
}