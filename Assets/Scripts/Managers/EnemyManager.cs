using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static List<Enemy> Enemies { get; private set; } = new();

    public static void Register(Enemy enemy) => Enemies.Add(enemy);
    public static void Unregister(Enemy enemy) => Enemies.Remove(enemy);
}
