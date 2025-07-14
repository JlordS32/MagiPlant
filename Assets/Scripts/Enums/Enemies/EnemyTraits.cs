[System.Flags]
public enum EnemyTrait
{
    None = 0,
    Stealth = 1 << 0,
    Flying = 1 << 1,
    Fast = 1 << 2,
    Slow = 1 << 3,
    Tank = 1 << 4,
    LowHealth = 1 << 5,
    HighHealth = 1 << 6,
    HighDamage = 1 << 7,
    LowDamage = 1 << 8,
    GlassCannon = 1 << 9
}