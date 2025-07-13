using System.Collections.Generic;
using UnityEngine;

public class WaveBuilder : MonoBehaviour
{
    [Header("Enemy Pool")]
    [SerializeField] GameObject[] catalog;

    [Header("Tuning curves")]
    [SerializeField]
    AnimationCurve budgetCurve =      // points per wave
        AnimationCurve.Linear(0, 10, 50, 250);
    [SerializeField]
    AnimationCurve gapCurve =         // gap shrink over time
        AnimationCurve.Linear(0, 1.2f, 50, 0.3f);

    [SerializeField] int maxPerGroup = 8;              // cap burst size

    /// Build() is pure: give it a wave index, get back a Wave struct.
    public Wave Build(int waveIndex)
    {
        int budget = Mathf.RoundToInt(budgetCurve.Evaluate(waveIndex));
        float gap = gapCurve.Evaluate(waveIndex);

        List<Group> groups = new();

        while (budget > 0)
        {
            // pick random prefab and assign a “cost” (1 here, swap for tier-based if needed)
            var prefab = catalog[Random.Range(0, catalog.Length)];
            int cost = 1;

            int count = Mathf.Min(
                Random.Range(3, maxPerGroup + 1),
                budget / cost);

            if (count == 0) break;    // cannot afford anything else

            groups.Add(new Group
            {
                prefab = prefab,
                gap = gap,
                count = count
            });

            budget -= cost * count;
        }

        return new Wave { groups = groups.ToArray() };
    }
}