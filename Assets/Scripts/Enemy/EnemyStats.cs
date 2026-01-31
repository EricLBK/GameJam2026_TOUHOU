using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 10;
    public int pointsWorth = 100;

    [Header("Soul Drop")]
    public int soulsToDrop = 3;
    public int soulValue = 50;

    int hp;

    void Awake()
    {
        hp = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        hp -= amount;

        if (hp <= 0)
            Die();
    }

    void Die()
    {
        // 1) score
        if (ScoreKeeper.Instance != null)
            ScoreKeeper.Instance.AddKill(pointsWorth);

        // 2) souls
        if (ItemManager.Instance != null)
            ItemManager.Instance.SpawnSoulBurst(transform.position, soulsToDrop);

        // 3) remove enemy
        Destroy(gameObject);
    }
}
