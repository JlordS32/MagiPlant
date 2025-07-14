using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
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
        GameEventsManager.RaisePlayerStatUpdate(stat, _stats[stat]);
    }

    public void AddExp(float amount)
    {
        if (amount > 0)
        {
            _stats[PlayerStats.EXP] += amount;
        }
        CheckLevelUp();
        GameEventsManager.RaiseExpGainUpdate(_stats[PlayerStats.EXP], GetRequiredEXP(_stats[PlayerStats.Level]));
    }

    public float Get(PlayerStats stat) => _stats[stat];
    public void Set(PlayerStats stat, float value) { _stats[stat] = value; GameEventsManager.RaisePlayerStatUpdate(stat, _stats[stat]); }

    public float GetRequiredEXP(float level)
    {
        float baseExp = _config.baseStats.Find(stat => stat.Type == PlayerStats.EXP)?.Value ?? 10f;

        return baseExp * Mathf.Pow(_config.expLevelUpRate, level - 1);
    }

    public float ApplyDamage(float amount)
    {
        float defense = _stats[PlayerStats.Defense];
        float finalDamage = Mathf.Max(0, amount - defense);
        _stats[PlayerStats.HP] = Mathf.Max(0, _stats[PlayerStats.HP] - finalDamage);
        GameEventsManager.RaisePlayerStatUpdate(PlayerStats.HP, _stats[PlayerStats.HP]);

        return finalDamage;
    }

    public bool IsDead => _stats[PlayerStats.HP] <= 0;

    public void Reset()
    {
        foreach (var stats in _config.baseStats)
        {
            _stats[stats.Type] = stats.Value;
        }

        GameEventsManager.RaisePlayerStatReset();
    }

    void Upgrade()
    {
        float level = _stats[PlayerStats.Level];

        foreach (var gain in _config.perLevelGains)
        {
            float baseValue = _stats.ContainsKey(gain.Type) ? _stats[gain.Type] : 0f;
            float increase = gain.Increase * (level - 1);
            _stats[gain.Type] = baseValue + increase;
        }

        // Set MaxHP and restore full HP
        if (_stats.ContainsKey(PlayerStats.MaxHP))
            _stats[PlayerStats.HP] = _stats[PlayerStats.MaxHP];

        GameEventsManager.RaisePlayerStatUpgrade(_stats);
    }

    bool CheckLevelUp()
    {
        float currentExp = _stats[PlayerStats.EXP];
        float currentLevel = _stats[PlayerStats.Level];
        float requiredExp = GetRequiredEXP(currentLevel);

        bool leveledUp = false;
        while (currentExp >= requiredExp)
        {
            _stats[PlayerStats.Level]++;
            Upgrade();
                
            currentExp -= requiredExp;
            requiredExp = GetRequiredEXP(_stats[PlayerStats.Level]);
            leveledUp = true;
            GameEventsManager.RaiseLevelUpUpdate((int)_stats[PlayerStats.Level]);
        }

        _stats[PlayerStats.EXP] = currentExp;
        return leveledUp;
    }
}
