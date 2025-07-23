using System;
using UnityEngine;


[Serializable]
public class EnemyData : Data<EnemyStats, EnemyStatConfig>
{
    public EnemyData(EnemyStatConfig config)
    {
        Init(config, DebugCategory.Enemies);
    }

    public float ApplyDamage(float amount)
    {
        float defense = _data[EnemyStats.Defense];
        float finalDamage = Mathf.Max(0, amount - defense);
        _data[EnemyStats.HP] = Mathf.Max(0, _data[EnemyStats.HP] - finalDamage);

        RaiseStatUpdateEvent(EnemyStats.HP, _data[EnemyStats.HP]);
        return finalDamage;
    }

    public bool IsDead => _data[EnemyStats.HP] <= 0;

    protected override void RaiseStatUpdateEvent(EnemyStats stat, float value)
    {
        GameEventsManager.RaiseEnemyStatUpdate(stat, value);
    }
    protected override void RaiseResetUpdateEvent()
    {
        GameEventsManager.RaiseEnemyStatReset();
    }
}
