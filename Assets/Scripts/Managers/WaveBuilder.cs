using System.Collections.Generic;
using UnityEngine;

public class WaveBuilder : MonoBehaviour
{
    [Header("Enemy Pool")]
    [SerializeField] EnemyCatalog _gruntCatalog;

    [Header("Tuning curves")]
    [SerializeField]
    AnimationCurve budgetCurve =      // points per wave
        AnimationCurve.Linear(0, 10, 50, 250);
    [SerializeField]
    AnimationCurve gapCurve =         // gap shrink over time
        AnimationCurve.Linear(0, 1.2f, 50, 0.3f);

    [SerializeField] int maxPerGroup = 8;              // cap burst size

    public Wave Build(int waveIndex)
    {
        int budget = Mathf.RoundToInt(budgetCurve.Evaluate(waveIndex));
        float gap = gapCurve.Evaluate(waveIndex);

        List<EnemyGroup> groups = new();

        while (budget > 0)
        {
            // Randomly select enemy
            EnemyEntry enemy = _gruntCatalog.entries[Random.Range(0, _gruntCatalog.entries.Length)];

            int count = Mathf.Min(
                Random.Range(3, maxPerGroup + 1),
                budget / enemy.Cost);

            if (count == 0) break;

            groups.Add(new EnemyGroup
            {
                prefab = enemy.EnemyPrefab,
                gap = gap,
                count = count
            });

            budget -= enemy.Cost * count;
        }

        return new Wave { groups = groups.ToArray() };
    }
}