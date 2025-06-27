using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // CONSTANTS
    const float DEFAULT_LEVEL = 1;
    const float DEFAULT_EXP = 0;
    const float DEFAULT_HP = 100;
    const float DEFAULT_ATTACK = 10;
    const float DEFAULT_DEFENSE = 5;
    const float DEFAULT_SPEED = 5;

    Dictionary<PlayerStats, float> _stats = new();
    PlayerStatConfig _config;

    public PlayerData(PlayerStatConfig config)
    {
        _config = config;

        foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
            _stats[stat] = 0f;

        Reset();
    }


    public void AddStats(PlayerStats stat, float amount)
    {
        // Only take non-negatives
        if (amount > 0 && stat != PlayerStats.EXP)
        {
            _stats[stat] += amount;
        }
    }

    public void AddExp(float amount)
    {
        if (amount > 0)
        {
            _stats[PlayerStats.EXP] += amount;
            CheckLevelUp();
        }
    }

    public float Get(PlayerStats stat) => _stats[stat];
    public void Set(PlayerStats stat, float value) => _stats[stat] = value;
    public float GetRequiredEXP(float level)
    {
        return _config.baseExp * Mathf.Pow(_config.expLevelUpRate, level - 1);
    }

    public void Reset()
    {
        _stats[PlayerStats.Level] = DEFAULT_LEVEL;
        _stats[PlayerStats.EXP] = DEFAULT_EXP;
        _stats[PlayerStats.HP] = DEFAULT_HP;
        _stats[PlayerStats.Attack] = DEFAULT_ATTACK;
        _stats[PlayerStats.Defense] = DEFAULT_DEFENSE;
        _stats[PlayerStats.Speed] = DEFAULT_SPEED;
    }

    bool CheckLevelUp()
    {
        float currentExp = _stats[PlayerStats.EXP];
        float currentLevel = _stats[PlayerStats.Level];
        float requiredExp = GetRequiredEXP(currentLevel);

        bool leveledUp = false;
        while (currentExp >= requiredExp)
        {
            currentExp -= requiredExp;
            _stats[PlayerStats.Level]++;
            requiredExp = GetRequiredEXP(_stats[PlayerStats.Level]);
            leveledUp = true;
        }

        _stats[PlayerStats.EXP] = currentExp;
        return leveledUp;
    }

}
