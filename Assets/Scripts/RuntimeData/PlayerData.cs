using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public static event System.Action<Dictionary<PlayerStats, float>> OnPlayerStatUpdate;

    // REFERENCES
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
        OnPlayerStatUpdate?.Invoke(_stats);
    }

    public void AddExp(float amount)
    {
        if (amount > 0)
        {
            _stats[PlayerStats.EXP] += amount;
        }
        OnPlayerStatUpdate?.Invoke(_stats);
    }

    public float Get(PlayerStats stat) => _stats[stat];
    public void Set(PlayerStats stat, float value) { _stats[stat] = value; OnPlayerStatUpdate?.Invoke(_stats); }

    public float GetRequiredEXP(float level)
    {
        return _config.baseExp * Mathf.Pow(_config.expLevelUpRate, level - 1);
    }

    public float ApplyDamage(float amount)
    {
        float defense = _stats[PlayerStats.Defense];
        float finalDamage = Mathf.Max(0, amount - defense);
        _stats[PlayerStats.HP] = Mathf.Max(0, _stats[PlayerStats.HP] - finalDamage);
        OnPlayerStatUpdate?.Invoke(_stats);

        return finalDamage;
    }

    public bool IsDead => _stats[PlayerStats.HP] <= 0;

    public void Reset()
    {
        _stats[PlayerStats.Level] = 1;
        _stats[PlayerStats.EXP] = 0;
        _stats[PlayerStats.MaxHP] = _config.baseHP;
        _stats[PlayerStats.HP] = _config.baseHP;
        _stats[PlayerStats.Attack] = _config.baseAttack;
        _stats[PlayerStats.Defense] = _config.baseDefense;
        OnPlayerStatUpdate?.Invoke(_stats);
    }

    void Upgrade()
    {
        float level = _stats[PlayerStats.Level];

        _stats[PlayerStats.MaxHP] = _config.baseHP + (20f * (level - 1));
        _stats[PlayerStats.HP] = _stats[PlayerStats.MaxHP]; // restore full HP on upgrade
        _stats[PlayerStats.Attack] = _config.baseAttack + (5f * (level - 1));
        _stats[PlayerStats.Defense] = _config.baseDefense + (2.5f * (level - 1));
        OnPlayerStatUpdate?.Invoke(_stats);
    }

    public bool CheckLevelUp()
    {
        float currentExp = _stats[PlayerStats.EXP];
        float currentLevel = _stats[PlayerStats.Level];
        float requiredExp = GetRequiredEXP(currentLevel);

        bool leveledUp = false;
        while (currentExp >= requiredExp)
        {
            Upgrade();
            currentExp -= requiredExp;
            _stats[PlayerStats.Level]++;
            requiredExp = GetRequiredEXP(_stats[PlayerStats.Level]);
            leveledUp = true;
        }

        _stats[PlayerStats.EXP] = currentExp;
        return leveledUp;
    }
}
