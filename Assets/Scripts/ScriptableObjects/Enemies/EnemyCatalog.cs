using UnityEngine;

[CreateAssetMenu(menuName = "SO/Enemies/Enemy Catalog")]
public class EnemyCatalog : ScriptableObject
{
    public EnemyEntry[] entries;
}