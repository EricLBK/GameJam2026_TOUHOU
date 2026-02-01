using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private SoulPickup soulPrefab;
    [SerializeField] private ExtraLifePickup extraLifePrefab;
    [SerializeField] private BombPickup bombPrefab;
    

    [Header("Spawn Tuning")]
    [SerializeField] private float burstRadius = 10f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SpawnSoul(Vector2 pos)
    {
        Instantiate(soulPrefab, pos, Quaternion.identity);
    }

    public void SpawnSoulBurst(Vector2 pos, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * burstRadius;
            SpawnSoul(pos + offset);
        }
    }

    public void SpawnExtraLife(Vector2 pos)
    {
        if (extraLifePrefab == null) return;
        Instantiate(extraLifePrefab, pos, Quaternion.identity);
    }

    public void SpawnBomb(Vector2 pos)
    {
        if (bombPrefab == null) return;
        Instantiate(bombPrefab, pos, Quaternion.identity);
    }
}
