using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tower Defense List")]
public class TowerConfig : ScriptableObject
{
    public DefenseEntry[] DefenseEntry;
}
