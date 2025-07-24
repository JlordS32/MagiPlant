using UnityEngine;
using System;

public class StatDebugger : MonoBehaviour
{
    [SerializeField] StatConfig config; // Non-generic base
    [SerializeField] int startLevel = 1;
    [SerializeField] int endLevel = 10;
    [SerializeField] string statName;

    [ContextMenu("Print Stat Range")]
    void PrintStatRange()
    {
        if (config == null)
        {
            Debug.LogWarning("No config assigned.");
            return;
        }

        Type enumType = config.GetType().BaseType.GetGenericArguments()[0];
        if (Enum.TryParse(enumType, statName, true, out object result))
        {
            config.PrintValuesInRange((Enum)result, startLevel, endLevel);
        }
        else
        {
            Debug.LogWarning($"'{statName}' is not a valid {enumType.Name}.");
        }
    }
}
