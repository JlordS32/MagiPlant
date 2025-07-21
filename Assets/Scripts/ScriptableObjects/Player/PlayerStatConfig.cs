using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BaseStat
{
    public PlayerStats Type;
    public float Value;
}

[Serializable]
public class Multiplier
{
    public PlayerStats Type;
    public float Increase;
}

//TODO[JAYLOU]: Update the stat config so it's more configurable like Resource and Tower.
//TODO: Centralised required exp in this config using animation curve.
[CreateAssetMenu(menuName = "SO/Player/Player Stat Config")]
public class PlayerStatConfig : ScriptableObject
{
    [Header("Base Stats")]
    public List<BaseStat> baseStats;

    [Header("Levels")]
    public float expLevelUpRate = 2.5f;
    public int maxLevel = 100;

    [Header("Rates per level")]
    public List<Multiplier> perLevelGains;
}
