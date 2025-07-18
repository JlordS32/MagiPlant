using UnityEngine;

[CreateAssetMenu(menuName = "SO/Enemies/Enemy Stat Config")]
public class EnemyStatConfig : ScriptableObject
{
    public float HP = 50;
    public float Attack = 10;
    public float Defense = 3;
    public float Speed = 2;
}
