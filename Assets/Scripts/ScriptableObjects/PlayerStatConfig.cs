using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Stat Config")]
public class PlayerStatConfig : ScriptableObject
{
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
