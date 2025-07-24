using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player/Player Stat Config")]
public class PlayerStatConfig : StatConfig<PlayerStats>
{
    public float GetRequiredExp(int level)
    {
        return GetValue(PlayerStats.EXP, level);
    }
}
