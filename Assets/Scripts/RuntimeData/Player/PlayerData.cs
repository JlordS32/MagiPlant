using System;
using UnityEngine;

[Serializable]
public class PlayerData : Data<PlayerStats, PlayerStatConfig>
{
    // VARIABLES
    float _minDamage = 1f;

    public PlayerData(PlayerStatConfig config)
    {
        Init(config);
    }

    public void AddStats(PlayerStats stat, float amount)
    {
        // Only take non-negatives
        if (amount > 0 && stat != PlayerStats.EXP)
        {
            _data[stat] += amount;
        }
        RaiseStatUpdateEvent(stat, _data[stat]);
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

    public float ApplyDamage(float amount)
    {
        float defense = _data[PlayerStats.Defense];
        float finalDamage = Mathf.Max(_minDamage, amount - defense);
        _data[PlayerStats.HP] = Mathf.Max(0, _data[PlayerStats.HP] - finalDamage);
        RaiseStatUpdateEvent(PlayerStats.HP, _data[PlayerStats.HP]);

        return finalDamage;
    }

    public bool IsDead => _data[PlayerStats.HP] <= 0;

    public void Upgrade(PlayerStats stat)
    {
        float newValue = _config.GetValue(stat, _level);
        _data[stat] = newValue;

        RaiseStatUpdateEvent(stat, newValue);
    }

    public void UpgradeAll()
    {
        if (_level >= _config.MaxLevel)
        {
            Debugger.LogWarning(_config.DebugCategory, "Attempting to level up beyond max level.");
            return;
        }

        _level++;

        foreach (var entry in _config.Stats)
        {
            Upgrade(entry.Stat);
            Debugger.Log(_config.DebugCategory, $"Level {_level}: {entry.Stat}: {_data[entry.Stat]}");
        }

        // Set MaxHP and restore full HP
        if (_data.ContainsKey(PlayerStats.MaxHP))
            _data[PlayerStats.HP] = _data[PlayerStats.MaxHP];
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
            UpgradeAll();

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
        foreach (PlayerStats stat in Enum.GetValues(typeof(PlayerStats)))
        {
            // Skip HP — set it later based on MaxHP
            if (stat == PlayerStats.HP)
                continue;

            _data[stat] = _config.GetValue(stat, 0);
        }

        // Ensure HP is synced to MaxHP
        _data[PlayerStats.HP] = _data[PlayerStats.MaxHP];

        _level = 1;
        RaiseResetUpdateEvent();
    }
}
