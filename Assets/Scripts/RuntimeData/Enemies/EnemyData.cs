using System;
using UnityEngine;

[Serializable]
public class EnemyData : DamageableData<EnemyStats, EnemyStatConfig>
{
    protected override EnemyStats HPStat => EnemyStats.HP;
    protected override EnemyStats DefenseStat => EnemyStats.Defense;

    public EnemyData(EnemyStatConfig config)
    {
        Init(config);
    }
    
    protected override void RaiseStatUpdateEvent(EnemyStats stat, float value)
    {
        GameEventsManager.RaiseEnemyStatUpdate(stat, value);
    }
    protected override void RaiseResetUpdateEvent()
    {
        GameEventsManager.RaiseEnemyStatReset();
    }

    public override void Reset()
    {
        base.Reset();

        // Ensure HP is synced to MaxHP
        _data[EnemyStats.HP] = _data[EnemyStats.MaxHP];

        _level = 1;
        RaiseResetUpdateEvent();
    }
}
