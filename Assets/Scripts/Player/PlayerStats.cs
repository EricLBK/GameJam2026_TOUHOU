using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Starting Values")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int startingBombs = 2;

    [Header("Maximums")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private int maxBombs = 5;

    [Header("Pickup")]
    public float pickupRadius = 40f;

    // Runtime values (read-only from outside)
    public int lives { get; private set; }
    public int bombs { get; private set; }
    public int souls { get; private set; }

    // Events
    public event Action<int> OnLivesChanged;
    public event Action<int> OnBombsChanged;
    public event Action<int> OnSoulsChanged;

    // Optional: one event to refresh *all* UI in one go
    public event Action OnStatsChanged;

    private bool initialized = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize player stats once per run.
    /// </summary>
    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        lives = Mathf.Clamp(startingLives, 0, maxLives);
        bombs = Mathf.Clamp(startingBombs, 0, maxBombs);
        souls = 0;

        // Fire events so UI updates immediately
        OnLivesChanged?.Invoke(lives);
        OnBombsChanged?.Invoke(bombs);
        OnSoulsChanged?.Invoke(souls);
        OnStatsChanged?.Invoke();
    }

    // ---------- Public API ----------

    public void AddLife(int amount = 1)
    {
        Debug.Log($"[Pickup] modifying stats instance={this.GetInstanceID()}");

        if (amount <= 0) return;

        int prev = lives;
        lives = Mathf.Clamp(lives + amount, 0, maxLives);

        if (lives != prev)
        {
            Debug.Log(lives+" many lives");
            OnLivesChanged?.Invoke(lives);
            OnStatsChanged?.Invoke();
        }
    }

    public void LoseLife(int amount = 1)
    {
        if (amount <= 0) return;

        int prev = lives;
        lives = Mathf.Max(0, lives - amount);

        if (lives != prev)
        {
            OnLivesChanged?.Invoke(lives);
            OnStatsChanged?.Invoke();
        }
    }

    public void AddBomb(int amount = 1)
    {
        Debug.Log($"[Pickup] modifying stats instance={this.GetInstanceID()}");
        if (amount <= 0) return;

        int prev = bombs;
        bombs = Mathf.Clamp(bombs + amount, 0, maxBombs);

        if (bombs != prev)
        {
            Debug.Log(bombs+" many bombs");
            OnBombsChanged?.Invoke(bombs);
            OnStatsChanged?.Invoke();
        }
    }

    public bool ConsumeBomb(int amount = 1)
    {
        if (amount <= 0) return true;
        if (bombs < amount) return false;

        bombs -= amount;
        OnBombsChanged?.Invoke(bombs);
        OnStatsChanged?.Invoke();
        return true;
    }

    public void AddSouls(int amount)
    {
        Debug.Log($"[Pickup] modifying stats instance={this.GetInstanceID()}");
        if (amount <= 0) return;

        souls += amount;
        OnSoulsChanged?.Invoke(souls);
        OnStatsChanged?.Invoke();
    }

    // ---------- Debug / Reset ----------

    public void ResetStats()
    {
        initialized = false;
        Initialize();
    }
}
