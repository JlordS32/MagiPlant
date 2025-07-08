using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventsManager
{
    #region EVENTS
    // Player
    public static event Action<PlayerStats, float> OnPlayerStatUpdate;
    public static event Action<Dictionary<PlayerStats, float>> OnPlayerStatUpgrade;
    public static event Action<Dictionary<PlayerStats, float>> OnPlayerStatReset;
    public static event Action<int> OnLevelUpUpdate;

    // Currencies
    public static event Action<Dictionary<CurrencyType, Storage>> OnCurrencyUpdate;
    #endregion

    #region TRIGGERS
    public static void RaisePlayerStatUpdate(PlayerStats stat, float value)
        => OnPlayerStatUpdate?.Invoke(stat, value);

    public static void RaisePlayerStatUpgrade(Dictionary<PlayerStats, float> upgrades)
        => OnPlayerStatUpgrade?.Invoke(upgrades);

    public static void RaisePlayerStatReset(Dictionary<PlayerStats, float> stats)
        => OnPlayerStatReset?.Invoke(stats);

    public static void RaiseLevelUpUpdate(int level)
        => OnLevelUpUpdate?.Invoke(level);

    public static void RaiseCurrencyUpdate(Dictionary<CurrencyType, Storage> currencies)
        => OnCurrencyUpdate?.Invoke(currencies);
    #endregion
}
