using System.Collections.Generic;
using UnityEngine;

public class WaveBuilder : MonoBehaviour
{
    [Header("Enemy Pool")]
    [SerializeField] EnemyCatalog _gruntCatalog;

    [Header("Tuning curves")]
    [SerializeField]
    AnimationCurve budgetCurve =
        AnimationCurve.Linear(0, 10, 10, 250);
    [SerializeField]
    AnimationCurve gapCurve =
        AnimationCurve.Linear(0, 1.2f, 10, 0.3f);

    [SerializeField] int maxPerGroup = 8;

    public Wave Build(int waveIndex)
    {
        if (_gruntCatalog == null || _gruntCatalog.entries.Length == 0)
        {
            Debugger.LogError(DebugCategory.Waves, "Grunt catalog is empty or not assigned.");
            return new Wave();
        }

        if (waveIndex < 0)
        {
            Debugger.LogError(DebugCategory.Waves, "Wave index cannot be negative.");
            return new Wave();
        }

        int budget = Mathf.RoundToInt(budgetCurve.Evaluate(waveIndex));
        float gap = gapCurve.Evaluate(waveIndex);

        Debugger.Log(DebugCategory.Waves, "Current Wave: " + waveIndex);
        Debugger.Log(DebugCategory.Waves, "Current Budget: " + budget);
        Debugger.Log(DebugCategory.Waves, "Current gap: " + gap);

        List<EnemyGroup> groups = new();

        while (budget > 0)
        {
            // Randomly select enemy
            EnemyEntry enemy = _gruntCatalog.entries[Random.Range(0, _gruntCatalog.entries.Length)];
            if (enemy.Cost <= 0)
            {
                Debugger.LogWarning(DebugCategory.Waves, $"{enemy.EnemyPrefab.name} has cost 0. Skipping. Please check the catalog.");
                continue;
            }

            int count = Mathf.Min(
                Random.Range(3, maxPerGroup + 1),
                budget / enemy.Cost);

            if (count == 0) break;

            groups.Add(new EnemyGroup
            {
                Prefab = enemy.EnemyPrefab,
                Gap = gap,
                Count = count
            });

            budget -= enemy.Cost * count;
        }

        return new Wave { Groups = groups.ToArray() };
    }
}