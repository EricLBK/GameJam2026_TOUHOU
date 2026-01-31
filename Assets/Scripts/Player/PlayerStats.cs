using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Pickup")]
    [Min(0f)] public float pickupRadius = 40f;

    [Header("Resources")]
    public int lives = 3;
    public int bombs = 2;

    [Header("Soul / Score")]
    public long score = 0;
    public int souls = 0;

    public event Action<int> OnLivesChanged;
    public event Action<int> OnBombsChanged;
    public event Action<long> OnScoreChanged;
    public event Action<int> OnSoulsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddSouls(int amount)
    {
        if (amount <= 0) return;
        souls += amount;
        OnSoulsChanged?.Invoke(souls);
    }

    public void AddScore(long amount)
    {
        if (amount <= 0) return;
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    public void AddLife(int amount = 1)
    {
        if (amount <= 0) return;
        lives += amount;
        OnLivesChanged?.Invoke(lives);
    }

    public void AddBomb(int amount = 1)
    {
        if (amount <= 0) return;
        bombs += amount;
        OnBombsChanged?.Invoke(bombs);
    }

    public bool ConsumeBomb(int amount = 1)
    {
        if (amount <= 0) return true;
        if (bombs < amount) return false;
        bombs -= amount;
        OnBombsChanged?.Invoke(bombs);
        return true;
    }
}
