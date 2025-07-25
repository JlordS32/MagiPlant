using System;
using System.Linq;

[Serializable]
public class TowerData : Data<TowerStats, TowerStatConfig>
{
    public TowerData(TowerStatConfig config)
    {
        Init(config);
    }
    
    protected override bool CanUpgrade()
    {
        float cost = _config.GetCost(_level);
        var currencies = _config.CostType;

        Debugger.Log(DebugCategory.Resources, $"Current Cost: {cost}");

        return currencies.All(c => CurrencyStorage.Instance.CanAfford(c, cost));
    }

    protected override void PostUpgrade()
    {
        float cost = _config.GetCost(_level);
        foreach (var currency in _config.CostType)
        {
            CurrencyStorage.Instance.Spend(currency, cost);
        }
    }

    protected override void RaiseStatUpdateEvent(TowerStats stat, float value)
    {
        GameEventsManager.RaiseTowerStatUpdate(stat, value);
    }

    protected override void RaiseResetUpdateEvent()
    {
        GameEventsManager.RaiseTowerStatReset();
    }
}
