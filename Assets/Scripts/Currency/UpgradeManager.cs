using UnityEngine;

// FIXME: Link the upgradenetry scriptable object and instantiate from here
// TODO: Add event listeners for any upgrade change.
public class UpgradeManager : MonoBehaviour
{
    // // REFERENCES
    // CurrencyStorage _storage;

    // void Awake() => _storage = GetComponent<CurrencyStorage>();

    // public void TryUpgrade(UpgradeEntry entry, IUpgradableCurrency target, CurrencyType type)
    // {
    //     if (entry.Level >= entry.MaxLevel)
    //         return;

    //     float cost = entry.GetCost();

    //     if (_storage.Spend(CurrencyType.Sunlight, cost))
    //     {
    //         entry.Upgrade();                         // Increase level
    //         target.ApplyUpgrade(entry, type);        // Apply effect
    //     }
    // }
}
