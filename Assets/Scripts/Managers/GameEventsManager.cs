using System;
using System.Collections.Generic;

public static class GameEventsManager
{
    #region EVENTS
    // Player
    public static event Action<PlayerStats, float> OnPlayerStatUpdate;
    public static event Action<Dictionary<PlayerStats, float>> OnPlayerStatUpgrade;
    public static event Action<int> OnLevelUpUpdate;
    public static event Action<float, float> OnExpGainUpdate;
    public static event Action OnPlayerStatReset;

    // Enemies
    public static event Action<EnemyStats, float> OnEnemyDataUpdate;
    public static event Action OnEnemyStatReset;

    // Tower Defenses
    public static event Action<TowerStats, float> OnTowerStatUpdate;
    public static event Action OnTowerStatReset;

    // Currencies
    public static event Action<CurrencyType, float> OnCurrencyUpdate;
    public static event Action OnCurrencyReset;

    // Click Rate 
    public static event Action<CurrencyType, float, int> OnClickRateUpdated;

    // Generate Rate 
    public static event Action<CurrencyType, float, int> OnGenerateRateUpdated;
    #endregion

    #region TRIGGERS
    // Player
    public static void RaisePlayerStatUpdate(PlayerStats stat, float value)
        => OnPlayerStatUpdate?.Invoke(stat, value);

    public static void RaisePlayerStatUpgrade(Dictionary<PlayerStats, float> upgrades)
        => OnPlayerStatUpgrade?.Invoke(upgrades);

    public static void RaiseLevelUpUpdate(int level)
        => OnLevelUpUpdate?.Invoke(level);

    public static void RaiseExpGainUpdate(float exp, float cap)
        => OnExpGainUpdate?.Invoke(exp, cap);

    public static void RaisePlayerStatReset()
        => OnPlayerStatReset?.Invoke();

    // Enemies
    public static void RaiseEnemyStatUpdate(EnemyStats type, float value)
        => OnEnemyDataUpdate?.Invoke(type, value);

    public static void RaiseEnemyStatReset()
        => OnEnemyStatReset?.Invoke();

    // Tower Defenses
    public static void RaiseTowerStatUpdate(TowerStats type, float value)
        => OnTowerStatUpdate?.Invoke(type, value);
    public static void RaiseTowerStatReset()
        => OnTowerStatReset?.Invoke();

    // Currency
    public static void RaiseCurrencyUpdate(CurrencyType type, float value)
        => OnCurrencyUpdate?.Invoke(type, value);

    public static void RaiseCurrencyReset()
        => OnCurrencyReset?.Invoke();

    // Click Rate 
    public static void RaiseClickRateUpdated(CurrencyType type, float rate, int level)
        => OnClickRateUpdated?.Invoke(type, rate, level);
        
    // Generate Rate
    public static void RaiseGenerateRateUpdated(CurrencyType type, float rate, int level)
        => OnGenerateRateUpdated?.Invoke(type, rate, level);
    #endregion
}
