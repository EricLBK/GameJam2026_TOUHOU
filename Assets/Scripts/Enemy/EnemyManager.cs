using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    readonly List<EnemyStats> enemies = new();
    public IReadOnlyList<EnemyStats> Enemies => enemies;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(EnemyStats e) => enemies.Add(e);
    public void Unregister(EnemyStats e) => enemies.Remove(e);
}
