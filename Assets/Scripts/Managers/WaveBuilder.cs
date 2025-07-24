using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class CatalogEntry
{
    public string Name;
    public EnemyTier Tier;
    public EnemyCatalog Catalog;
    public AnimationCurve CountCurve;
    public int SpawnInEveryWave = 1;
}

public class WaveBuilder : MonoBehaviour
{
    [Header("Enemy Pool")]
    [SerializeField] CatalogEntry[] _catalogEntries;

    [Header("Tuning curves")]
    [SerializeField]
    AnimationCurve budgetCurve =
        AnimationCurve.Linear(0, 10, 10, 250);
    [SerializeField]
    AnimationCurve gapCurve =
        AnimationCurve.Linear(0, 0.3f, 10, 0.1f);

    [SerializeField] int maxPerGroup = 8;

    public Wave Build(int waveIndex)
    {
        if (waveIndex < 0)
        {
            Debugger.LogError(DebugCategory.Waves, "Wave index cannot be negative.");
            return new Wave();
        }

        int budget = Mathf.RoundToInt(budgetCurve.Evaluate(waveIndex));
        float gap = gapCurve.Evaluate(waveIndex);
        List<EnemyGroup> groups = new();

        // Try to spawn higher tier enemies first.
        foreach (var entry in _catalogEntries
            .Where(e => e.Tier >= EnemyTier.Rare)
            .OrderByDescending(e => e.Tier))
        {
            TrySpawnFromCatalog(entry, waveIndex, gap, groups, ref budget);
        }

        // Then spawn grunts
        var gruntEntries = _catalogEntries
            .Where(e => e.Tier == EnemyTier.Grunt && e.Catalog != null && e.Catalog.entries.Length > 0)
            .ToArray();
        
        foreach (var entry in gruntEntries)
        {
            TrySpawnFromCatalog(entry, waveIndex, gap, groups, ref budget);
            if (budget <= 0) break;
        }


        return new Wave { Groups = groups.ToArray() };
    }

    private void TrySpawnFromCatalog(CatalogEntry entry, int waveIndex, float gap, List<EnemyGroup> groups, ref int budget)
    {
        // Skip if entry is not meant to spawn in this wave.
        if (entry.SpawnInEveryWave > 1 && waveIndex % entry.SpawnInEveryWave != 0)
            return;

        int count = Mathf.RoundToInt(entry.CountCurve.Evaluate(waveIndex));
        if (count <= 0 || entry.Catalog == null || entry.Catalog.entries.Length == 0) return;

        var enemy = entry.Catalog.entries[UnityEngine.Random.Range(0, entry.Catalog.entries.Length)];
        int totalCost = enemy.Cost * count;

        if (enemy.Cost <= 0 || totalCost > budget) return;

        groups.Add(new EnemyGroup
        {
            Prefab = enemy.EnemyPrefab,
            Gap = gap,
            Count = count
        });

        budget -= totalCost;
    }
}