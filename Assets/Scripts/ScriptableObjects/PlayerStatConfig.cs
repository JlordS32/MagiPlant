using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Stat Config")]
public class PlayerStatConfig : ScriptableObject
{
    [Header("Base Stats")]
    public float baseHP = 100f;
    public float baseAttack = 10f;
    public float baseDefense = 5f;
    public float baseSpeed = 1f;
    
    [Header("EXP Settings")]
    public float expLevelUpRate = 2.5f;
    public float baseExp = 10f;

    [Header("Per Level Gains")]
    public float atkPerLevel = 2f;
    public float hpPerLevel = 5f;
    public float defPerLevel = 1f;
    public float spdPerLevel = 0.5f;

    [Header("Other Settings")]
    public int maxLevel = 100;
}
