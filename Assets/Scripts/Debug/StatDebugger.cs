using UnityEngine;
using System;

public class StatDebugger : MonoBehaviour
{
    [SerializeField] StatConfig _config; // Non-generic base
    [SerializeField] int _startLevel = 1;
    [SerializeField] int _endLevel = 10;
    [SerializeField] string _statName;
    [SerializeField] bool _cumulative;

    [ContextMenu("Debug/Stat Range")]
    void PrintStatRange()
    {
        if (_config == null)
        {
            Debug.LogWarning("No config assigned.");
            return;
        }

        Type enumType = _config.GetType().BaseType.GetGenericArguments()[0];
        if (Enum.TryParse(enumType, _statName, true, out object result))
        {
            _config.PrintValuesInRange((Enum)result, _startLevel, _endLevel, _cumulative);
        }
        else
        {
            Debug.LogWarning($"'{_statName}' is not a valid {enumType.Name}.");
        }
    }
}
