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

// We use point-based system for the endless wave spawning logic.
// We start with a budget that increases over time as wave index (number) increases.
// Each wave contains a list of Group (EnemyGroup), where each group may contain more than 
// one copy of themselves to give a sense of hunting in packs!
// TODO: Make grunt group be mixed with some other grunts.
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

    void Update()
    {
        Time.timeScale = 4f;
    }

    public Wave Build(int waveIndex)
    {
        if (waveIndex < 0)
        {
            Debugger.LogError(DebugCategory.Waves, "Wave index cannot be negative.");
            return new Wave();
        }

        int budget = Mathf.RoundToInt(budgetCurve.Evaluate(waveIndex));
        float gap = gapCurve.Evaluate(waveIndex);

        Debugger.Log(DebugCategory.Waves, $"Budget: {budget}");
        Debugger.Log(DebugCategory.Waves, $"Gap: {gap}");

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

        while (budget > 0)
        {
            // Pick a random grunt entry each time
            var entry = gruntEntries[UnityEngine.Random.Range(0, gruntEntries.Length)];
            int prevBudget = budget;

            TrySpawnFromCatalog(entry, waveIndex, gap, groups, ref budget);

            if (budget == prevBudget)
            {
                Debugger.LogWarning(DebugCategory.Waves, $"Stuck spawn detected. Budget = {budget}, no spawn from {entry.Name}. Breaking loop.");
                break; // Prevent freeze
            }
        }

        Debugger.Log(DebugCategory.Waves, $"Budget end of wave {waveIndex}: {budget}");
        return new Wave { Groups = groups.ToArray() };
    }

    private void TrySpawnFromCatalog(CatalogEntry entry, int waveIndex, float gap, List<EnemyGroup> groups, ref int budget)
    {
        if (entry.SpawnInEveryWave > 1 && waveIndex % entry.SpawnInEveryWave != 0 && waveIndex > 0)
            return;

        if (entry.Catalog == null || entry.Catalog.entries.Length == 0) return;

        // Get random enemies from the catalog
        var catalog = entry.Catalog;
        var enemy = catalog.entries[UnityEngine.Random.Range(0, catalog.entries.Length)];
        Debug.Log($"Catalog: {enemy.EnemyEntryName}");
        if (enemy.Cost <= 0) return;

        // Evaluate how many can be spawned in that catalog
        int totalCount = Mathf.RoundToInt(entry.CountCurve.Evaluate(waveIndex));
        if (totalCount <= 0) return;

        // Spawn
        while (totalCount > 0 && budget >= enemy.Cost)
        {
            int groupCount = Mathf.Min(UnityEngine.Random.Range(2, maxPerGroup + 1), totalCount, budget / enemy.Cost);
            if (groupCount <= 0) break;

            groups.Add(new EnemyGroup
            {
                Prefab = enemy.EnemyPrefab,
                Gap = gap,
                Count = groupCount
            });

            totalCount -= groupCount;
            budget -= groupCount * enemy.Cost;
        }
    }

}