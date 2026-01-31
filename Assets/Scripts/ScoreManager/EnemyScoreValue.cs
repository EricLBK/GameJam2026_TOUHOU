using UnityEngine;

public class EnemyScoreValue : MonoBehaviour
{
    [Header("Score")]
    public int baseScore = 100;

    [Header("Death FX")]
    [SerializeField] private GameObject enemyDeadPrefab;

    [Header("Soul Drop")]
    [SerializeField] private int soulsToDrop = 3;
    [SerializeField] private int soulValue = 50;

    public void Die()
    {
        // 1) score
        if (ScoreKeeper.Instance != null)
            ScoreKeeper.Instance.AddKill(baseScore);

        // 2) spawn souls
        if (ItemManager.Instance != null)
            ItemManager.Instance.SpawnSoulBurst(transform.position, soulsToDrop);

        // 3) death visual
        if (enemyDeadPrefab != null)
            Instantiate(enemyDeadPrefab, transform.position, Quaternion.identity);

        // 4) remove enemy
        Destroy(gameObject);
    }
}
