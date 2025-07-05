using UnityEngine;

public class Enemy : MonoBehaviour
{
    void OnEnable() => EnemyManager.Register(this);
    void OnDisable() => EnemyManager.Unregister(this);

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(10f);
        }
    }

}
