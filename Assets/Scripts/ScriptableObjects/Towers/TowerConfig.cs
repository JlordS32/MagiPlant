using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tower Defense List")]
public class TowerConfig : ScriptableObject
{
    public List<DefenseEntry> DefenseEntry;
}
