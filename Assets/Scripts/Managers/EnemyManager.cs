using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public List<Enemy> Enemies { get; private set; } = new();
    public int ActiveCount => Enemies.Count;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of EnemyManager detected. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(Enemy enemy)
    {
        Enemies.Add(enemy);
    }
    public void Unregister(Enemy enemy) => Enemies.Remove(enemy);
}
