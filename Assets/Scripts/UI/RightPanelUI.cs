using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelUI : MonoBehaviour
{
    [Header("Labels")]
    public TMP_Text stageText;
    public TMP_Text difficultyText;

    [Header("Score")]
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public TMP_Text multText;

    [Header("Player")]
    public TMP_Text soulsText;

    // Optional numeric counters (leave unassigned if you don't want them)
    public TMP_Text livesCountText;
    public TMP_Text bombsCountText;

    public Image[] lifeIcons;
    public Image[] bombIcons;

    [SerializeField] private string stageLabel = "Stage 1";
    [SerializeField] private string difficultyLabel = "NORMAL";

    private Transform player;
    private PlayerStats ps;

    void OnEnable()
    {
        

        if (stageText) stageText.text = stageLabel;
        if (difficultyText) difficultyText.text = difficultyLabel;

        
        // ScoreKeeper
        if (ScoreKeeper.Instance != null)
        {
            ScoreKeeper.Instance.OnScoreChanged += OnScore;
            ScoreKeeper.Instance.OnComboChanged += OnCombo;

            OnScore(ScoreKeeper.Instance.Score);
            OnCombo(ScoreKeeper.Instance.Combo, ScoreKeeper.Instance.Multiplier);
        }

        // PlayerStats
        // Re-grab PlayerStats here (safe even if Awake order changes)
        if (ps == null) ps = PlayerStats.Instance;
        Debug.Log($"[UI] subscribing to PlayerStats.Instance={ps.GetInstanceID()}");
        if (ps != null)
        {
            ps.OnLivesChanged += OnLives;
            ps.OnBombsChanged += OnBombs;
            ps.OnSoulsChanged += OnSouls;

            OnLives(ps.lives);
            OnBombs(ps.bombs);
            OnSouls(ps.souls);
        }
        else
        {
            Debug.LogWarning("RightPanelUI: PlayerStats.Instance is null. Lives/Bombs won't update until PlayerStats exists.");
        }
    }
    void Update()
    {
        if (ps == null)
        {
            ResolveRefs();
            return;
        }

    }

    void OnDisable()
    {
        if (ScoreKeeper.Instance != null)
        {
            ScoreKeeper.Instance.OnScoreChanged -= OnScore;
            ScoreKeeper.Instance.OnComboChanged -= OnCombo;
        }

        if (ps != null)
        {
            ps.OnLivesChanged -= OnLives;
            ps.OnBombsChanged -= OnBombs;
            ps.OnSoulsChanged -= OnSouls;
        }
    }

    void OnScore(long v) { if (scoreText) scoreText.text = $"{v:0000000000}"; }

    void OnCombo(int c, int m)
    {
        if (comboText) comboText.text = $"{c}";
        if (multText)  multText.text  = $"x{m}";
    }

    void OnSouls(int s) { if (soulsText) soulsText.text = $"{s}"; }

    void OnLives(int n)
    {
        SetIcons(lifeIcons, n);
        if (livesCountText) livesCountText.text = $"Life x{n}";
    }

    void OnBombs(int n)
    {
        SetIcons(bombIcons, n);
        if (bombsCountText) bombsCountText.text = $"Bomb x{n}";
    }

    static void SetIcons(Image[] icons, int count)
    {
        if (icons == null) return;
        for (int i = 0; i < icons.Length; i++)
            if (icons[i] != null)
                icons[i].gameObject.SetActive(i < count);

    }

    void ResolveRefs()
    {
        if (PlayerRegistry.PlayerTransform != null) player = PlayerRegistry.PlayerTransform;
        if (PlayerRegistry.PlayerStats != null) ps = PlayerRegistry.PlayerStats;

        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }
        if (ps == null && player != null)
            ps = player.GetComponent<PlayerStats>();

        if (ps == null) ps = PlayerStats.Instance;
    }

}
