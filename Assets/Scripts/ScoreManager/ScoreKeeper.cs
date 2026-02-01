using System;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static ScoreKeeper Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private long score = 0;

    [Header("Combo")]
    [SerializeField] private int combo = 0;
    [SerializeField] private float comboResetTime = 2.5f;
    private float comboTimer = 0f;

    [Header("Multiplier Tiers")]
    [SerializeField] private int[] comboThresholds = { 0, 10, 25, 50, 100, 150, 200, 300, 400,  500 };
    [SerializeField] private int[] multipliers     = { 1,  2,  3,  4,   5,   6,   7,   8,   9,   10 };

    public event Action<long> OnScoreChanged;
    public event Action<int, int> OnComboChanged; // combo, multiplier

    public long Score => score;
    public int Combo => combo;
    public int Multiplier => GetMultiplierForCombo(combo);

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Push initial values to UI
        OnScoreChanged?.Invoke(score);
        OnComboChanged?.Invoke(combo, Multiplier);
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

    public void AddScore(int baseValue)
    {
        if (baseValue <= 0) return;

        long add = (long)baseValue * Multiplier;
        score += add;

        OnScoreChanged?.Invoke(score);
        AddCombo(1);
        RefreshComboTimer();
    }

    public void ResetAll()
    {
        score = 0;
        combo = 0;
        comboTimer = 0f;

        OnScoreChanged?.Invoke(score);
        OnComboChanged?.Invoke(combo, Multiplier);
    }

    public void PlayerHit()
    {
        ResetCombo();
    }

    public void BombUsed()
    {
        int tier = GetTierIndex(combo);
        if (tier <= 0) return;

        combo = comboThresholds[tier - 1];
        RefreshComboTimer();
        OnComboChanged?.Invoke(combo, Multiplier);
    }

    // ----- Internal -----

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
        int idx = 0;
        for (int i = 0; i < comboThresholds.Length; i++)
        {
            if (c >= comboThresholds[i]) idx = i;
            else break;
        }
        return Mathf.Clamp(idx, 0, multipliers.Length - 1);
    }
}
