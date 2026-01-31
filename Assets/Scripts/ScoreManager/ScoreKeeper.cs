// ScoreManager.cs
// Drop this on an empty GameObject in your first scene (or make it a prefab).
// Then call ScoreManager.Instance.AddKill(baseScore), AddGraze(), AddPickup(value), PlayerHit(), BombUsed().

using System;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper Instance { get; private set; }

    [Header("Core")]
    [SerializeField] private long score = 0;
    [SerializeField] private int combo = 0;

    [Header("Combo Settings")]
    [Tooltip("Seconds without scoring before combo resets.")]
    [SerializeField] private float comboResetTime = 2.5f;
    private float comboTimer = 0f;

    [Header("Multiplier Tiers")]
    // combo thresholds must be increasing
    [SerializeField] private int[] comboThresholds = { 0, 10, 25, 50, 100 };
    [SerializeField] private int[] multipliers     = { 1,  2,  3,  4,   5 };

    [Header("Point Values")]
    [SerializeField] private int grazePoints = 10;
    [SerializeField] private int pickupSmallPoints = 50;
    [SerializeField] private int pickupBigPoints = 200;

    [Header("Penalties")]
    [Tooltip("On hit, reset combo and multiplier to x1.")]
    [SerializeField] private bool resetOnHit = true;

    [Tooltip("On bomb, drop multiplier by one tier.")]
    [SerializeField] private bool bombDropsOneTier = true;

    public event Action<long> OnScoreChanged;
    public event Action<int, int> OnComboChanged; // combo, multiplier

    public long Score => score;
    public int Combo => combo;
    public int Multiplier => GetMultiplierForCombo(combo);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        comboTimer = 0f;
        ValidateTiers();
    }

    private void Update()
    {
        if (combo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
                ResetCombo();
        }
    }

    // ----- Public API -----

    public void AddKill(int baseScore)
    {
        if (baseScore <= 0) return;

        int mult = Multiplier;
        long add = (long)baseScore * mult;
        AddScoreInternal(add);

        AddCombo(1);
        RefreshComboTimer();
    }

    public void AddGraze()
    {
        int mult = Multiplier;
        long add = (long)grazePoints * mult;
        AddScoreInternal(add);

        // grazing should help maintain chain but not inflate combo too hard
        AddCombo(1);
        RefreshComboTimer();
    }

    public void AddPickupSmall() => AddPickup(pickupSmallPoints);
    public void AddPickupBig()   => AddPickup(pickupBigPoints);

    public void AddPickup(int value)
    {
        if (value <= 0) return;

        int mult = Multiplier;
        long add = (long)value * mult;
        AddScoreInternal(add);

        // pickups refresh chain but add less combo
        AddCombo(1);
        RefreshComboTimer();
    }

    public void PlayerHit()
    {
        if (resetOnHit)
            ResetCombo();
    }

    public void BombUsed()
    {
        if (!bombDropsOneTier) return;

        // Reduce combo to just below previous tier threshold so multiplier drops 1 tier.
        int currentTier = GetTierIndex(combo);
        if (currentTier <= 0) return; // already x1

        int newTier = currentTier - 1;

        // Keep some combo, but ensure tier is reduced
        int newCombo = comboThresholds[newTier];
        combo = newCombo;

        RefreshComboTimer();
        OnComboChanged?.Invoke(combo, Multiplier);
    }

    public void AddBossPhaseClear(int basePhaseBonus, bool applyMultiplier = false)
    {
        if (basePhaseBonus <= 0) return;
        long add = applyMultiplier ? (long)basePhaseBonus * Multiplier : basePhaseBonus;
        AddScoreInternal(add);

        // Usually refresh chain so boss phases keep flow
        AddCombo(3);
        RefreshComboTimer();
    }

    public void AddTimeBonus(int secondsRemaining, int pointsPerSecond = 100)
    {
        if (secondsRemaining <= 0 || pointsPerSecond <= 0) return;
        AddScoreInternal((long)secondsRemaining * pointsPerSecond);
    }

    public void AddFlatBonus(int amount)
    {
        if (amount <= 0) return;
        AddScoreInternal(amount);
    }

    public void ResetAll()
    {
        score = 0;
        combo = 0;
        comboTimer = 0;
        OnScoreChanged?.Invoke(score);
        OnComboChanged?.Invoke(combo, Multiplier);
    }

    // ----- Internals -----

    private void AddScoreInternal(long amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    private void AddCombo(int amount)
    {
        combo = Mathf.Max(0, combo + amount);
        OnComboChanged?.Invoke(combo, Multiplier);
    }

    private void RefreshComboTimer()
    {
        comboTimer = comboResetTime;
    }

    private void ResetCombo()
    {
        combo = 0;
        comboTimer = 0f;
        OnComboChanged?.Invoke(combo, Multiplier);
    }

    private int GetMultiplierForCombo(int c)
    {
        int idx = GetTierIndex(c);
        return multipliers[idx];
    }

    private int GetTierIndex(int c)
    {
        // Find highest threshold <= combo
        int idx = 0;
        for (int i = 0; i < comboThresholds.Length; i++)
        {
            if (c >= comboThresholds[i]) idx = i;
            else break;
        }
        return Mathf.Clamp(idx, 0, multipliers.Length - 1);
    }

    private void ValidateTiers()
    {
        if (comboThresholds == null || multipliers == null ||
            comboThresholds.Length == 0 || multipliers.Length == 0 ||
            comboThresholds.Length != multipliers.Length)
        {
            Debug.LogWarning("ScoreManager: comboThresholds and multipliers must be non-empty and same length. Using defaults.");
            comboThresholds = new[] { 0, 10, 25, 50, 100 };
            multipliers = new[] { 1, 2, 3, 4, 5 };
        }

        for (int i = 1; i < comboThresholds.Length; i++)
        {
            if (comboThresholds[i] < comboThresholds[i - 1])
            {
                Debug.LogWarning("ScoreManager: comboThresholds must be increasing. Fixing by sorting.");
                Array.Sort(comboThresholds);
                break;
            }
        }
    }
}
