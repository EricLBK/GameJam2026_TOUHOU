using System.Collections.Generic;
using UnityEngine;

public class SoulManager : MonoBehaviour
{
    public static SoulManager Instance { get; private set; }

    [SerializeField] SoulPickup soulPrefab;
    [SerializeField] Transform player;
    [SerializeField] float playerPickupRadius = 40f;

    readonly List<SoulPickup> active = new();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Spawn(Vector2 pos, int value)
    {
        SoulPickup s = Instantiate(soulPrefab, pos, Quaternion.identity);
        s.value = value;
        s.Init(player, playerPickupRadius);
        active.Add(s);
        
    }

    public void SpawnBurst(Vector2 pos, int count, int value)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 0.2f;
            Spawn(pos + offset, value);
        }
    }

    public void Despawn(SoulPickup s)
    {
        active.Remove(s);
        Destroy(s.gameObject);
    }
}
