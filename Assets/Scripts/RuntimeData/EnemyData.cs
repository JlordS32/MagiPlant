using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyData
{
    Dictionary<EnemyStats, float> _stats = new();
    EnemyStatConfig _config;

    public EnemyData(EnemyStatConfig config)
    {
        _config = config;

        foreach (EnemyStats stat in Enum.GetValues(typeof(EnemyStats)))
            _stats[stat] = 0f;

        Reset();
    }

    public float Get(EnemyStats stat) => _stats[stat];
    public void Set(EnemyStats stat, float value) => _stats[stat] = value;

    public float ApplyDamage(float amount)
    {
        float defense = _stats[EnemyStats.Defense];
        float finalDamage = Mathf.Max(0, amount - defense);
        _stats[EnemyStats.HP] = Mathf.Max(0, _stats[EnemyStats.HP] - finalDamage);

        GameEventsManager.RaiseEnemyStatUpdate(EnemyStats.HP, _stats[EnemyStats.HP]);
        return finalDamage;
    }

    public bool IsDead => _stats[EnemyStats.HP] <= 0;

    public void Reset()
    {
        _stats[EnemyStats.HP] = _config.HP;
        _stats[EnemyStats.MaxHP] = _config.HP;
        _stats[EnemyStats.Attack] = _config.Attack;
        _stats[EnemyStats.Defense] = _config.Defense;
        _stats[EnemyStats.Speed] = _config.Speed;

        GameEventsManager.RaiseEnemyStatReset();
    }
}
