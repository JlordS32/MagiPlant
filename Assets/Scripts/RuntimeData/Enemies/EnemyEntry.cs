using UnityEngine;
using System;

[Serializable]
public class EnemyEntry
{
    public string EnemyEntryName;
    public GameObject EnemyPrefab;
    public EnemyTier Tier;
    public EnemyType Type;
    public EnemyTrait Traits;
    public int Cost = 1;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (EnemyPrefab &&
            EnemyPrefab.GetComponent<Enemy>() == null)
        {
            Debug.LogWarning($"{EnemyPrefab.name} isn’t a enemy prefab", EnemyPrefab);
            EnemyPrefab = null;
        }
    }
#endif
}