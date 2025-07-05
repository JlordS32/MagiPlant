using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnEnable() => EnemyManager.Register(this);
    void OnDisable() => EnemyManager.Unregister(this);
}
