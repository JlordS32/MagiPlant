using System;
using UnityEngine;

public abstract class DamageableData<TEnum, TConfig> : Data<TEnum, TConfig>
    where TEnum : Enum
    where TConfig : StatConfig<TEnum>
{
    protected virtual float MinDamage => 1f;
    protected abstract TEnum HPStat { get; }
    protected abstract TEnum DefenseStat { get; }

    public float ApplyDamage(float amount)
    {
        float defense = _data[DefenseStat];
        float final = Mathf.Max(MinDamage, amount - defense);
        _data[HPStat] = Mathf.Max(0, _data[HPStat] - final);
        RaiseStatUpdateEvent(HPStat, _data[HPStat]);
        return final;
    }

    public bool IsDead => _data[HPStat] <= 0;
}