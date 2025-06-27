using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField] GameObject _upgradeComponent;

    public void Build(List<UpgradeEntry> entries)
    {
        // Clean up parent folder first
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach (var entry in entries)
        {
            GameObject go = Instantiate(_upgradeComponent, transform);
            var ui = go.GetComponent<UpgradeUI>();
            entry.UIRef = ui;

            ui.Setup(
                entry.Name,
                entry.GetRate,
                entry.GetButtonLabel,
                entry.GetLevel,
                () =>
                {
                    entry.UpgradeLogic.Invoke();
                });
        }
    }

}
