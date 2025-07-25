using System;
using UnityEngine;

[Serializable]
public class PlayerData : DamageableData<PlayerStats, PlayerStatConfig>
{
    protected override PlayerStats HPStat => PlayerStats.HP;
    protected override PlayerStats DefenseStat => PlayerStats.Defense;

    public PlayerData(PlayerStatConfig config)
    {
        Init(config);
    }

    public void AddExp(float amount)
    {
        if (amount > 0)
        {
            _data[PlayerStats.EXP] += amount;
        }
        CheckLevelUp();
        GameEventsManager.RaiseExpGainUpdate(_data[PlayerStats.EXP], _config.GetRequiredExp(_level));
    }

    // For cheating
    public void LevelUp()
    {
        _data[PlayerStats.EXP] += _config.GetRequiredExp(_level);
        CheckLevelUp();
        GameEventsManager.RaiseExpGainUpdate(_data[PlayerStats.EXP], _config.GetRequiredExp(_level));
    }
    
    bool CheckLevelUp()
    {
        if (_level >= _config.MaxLevel) return false;

        float currentExp = _data[PlayerStats.EXP];
        float requiredExp = _config.GetRequiredExp(_level);

        bool leveledUp = false;
        while (currentExp >= requiredExp)
        {
            _level++;
            TryUpgradeAll();

            currentExp -= requiredExp;
            requiredExp = _config.GetRequiredExp(_level);
            leveledUp = true;
            GameEventsManager.RaiseLevelUpUpdate(_level);
        }

        _data[PlayerStats.EXP] = currentExp;
        return leveledUp;
    }

    protected override void RaiseStatUpdateEvent(PlayerStats stat, float value)
    {
        GameEventsManager.RaisePlayerStatUpdate(stat, value);
    }

    protected override void RaiseResetUpdateEvent()
    {
        GameEventsManager.RaisePlayerStatReset();
    }

    public override void Reset()
    {
        base.Reset();

        // Ensure HP is synced to MaxHP
        _data[PlayerStats.HP] = _data[PlayerStats.MaxHP];

        _level = 1;
        RaiseResetUpdateEvent();
    }
}
