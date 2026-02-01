// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;

// public class RightPanelUI : MonoBehaviour
// {
//     [Header("Labels")]
//     public TMP_Text stageText;
//     public TMP_Text difficultyText;

//     // [Header("Score")]
//     // public TMP_Text scoreText;
//     // public TMP_Text comboText;
//     // public TMP_Text multText;

//     [Header("Handwritten Digits")]
//     public DigitStripUI scoreDigits;
//     public DigitStripUI comboDigits;
//     public DigitStripUI multiplierDigits;
//     public DigitStripUI soulsDigits;
//     public DigitStripUI killsDigits;


//     [Header("Player")]
//     public TMP_Text soulsText;
//     public TMP_Text killsText;

//     // Optional numeric counters (leave unassigned if you don't want them)
//     public TMP_Text livesCountText;
//     public TMP_Text bombsCountText;

//     public Image[] lifeIcons;
//     public Image[] bombIcons;

//     [SerializeField] private string stageLabel = "Stage 1";
//     [SerializeField] private string difficultyLabel = "NORMAL";

//     private Transform player;
//     private PlayerStats ps;

//     void OnEnable()
//     {
        

//         if (stageText) stageText.text = stageLabel;
//         if (difficultyText) difficultyText.text = difficultyLabel;

        
//         // ScoreKeeper
//         if (ScoreKeeper.Instance != null)
//         {
//             ScoreKeeper.Instance.OnScoreChanged += OnScore;
//             ScoreKeeper.Instance.OnComboChanged += OnCombo;

//             OnScore(ScoreKeeper.Instance.Score);
//             OnCombo(ScoreKeeper.Instance.Combo, ScoreKeeper.Instance.Multiplier);
//         }

//         // PlayerStats
//         // Re-grab PlayerStats here (safe even if Awake order changes)
//         if (ps == null) ps = PlayerStats.Instance;
//         Debug.Log($"[UI] subscribing to PlayerStats.Instance={ps.GetInstanceID()}");
//         if (ps != null)
//         {
//             ps.OnLivesChanged += OnLives;
//             ps.OnBombsChanged += OnBombs;
//             ps.OnSoulsChanged += OnSouls;
//             ps.OnKillsChanged += OnKills;

//             OnLives(ps.lives);
//             OnBombs(ps.bombs);
//             OnSouls(ps.souls);
//             OnKills(ps.kills);
//         }
//         else
//         {
//             Debug.LogWarning("RightPanelUI: PlayerStats.Instance is null. Lives/Bombs won't update until PlayerStats exists.");
//         }
//     }
//     void Update()
//     {
//         if (ps == null)
//         {
//             ResolveRefs();
//             return;
//         }

//     }

//     void OnDisable()
//     {
//         if (ScoreKeeper.Instance != null)
//         {
//             ScoreKeeper.Instance.OnScoreChanged -= OnScore;
//             ScoreKeeper.Instance.OnComboChanged -= OnCombo;
//         }

//         if (ps != null)
//         {
//             ps.OnLivesChanged -= OnLives;
//             ps.OnBombsChanged -= OnBombs;
//             ps.OnSoulsChanged -= OnSouls;
//             ps.OnKillsChanged -= OnKills;
//         }
//     }

//     // void OnScore(long v) { if (scoreText) scoreText.text = $"{v:0000000000}"; }

//     // void OnCombo(int c, int m)
//     // {
//     //     if (comboText) comboText.text = $"{c}";
//     //     if (multText)  multText.text  = $"x{m}";
//     // }

//     // void OnSouls(int s) { if (soulsText) soulsText.text = $"{s}"; }
//     // void OnKills(int s) { if (killsText) killsText.text = $"{s}"; }

//     void OnLives(int n)
//     {
//         SetIcons(lifeIcons, n);
//         if (livesCountText) livesCountText.text = $"Life x{n}";
//     }

//     void OnBombs(int n)
//     {
//         SetIcons(bombIcons, n);
//         if (bombsCountText) bombsCountText.text = $"Bomb x{n}";
//     }

//     static void SetIcons(Image[] icons, int count)
//     {
//         if (icons == null) return;
//         for (int i = 0; i < icons.Length; i++)
//             if (icons[i] != null)
//                 icons[i].gameObject.SetActive(i < count);

//     }

//     void ResolveRefs()
//     {
//         if (PlayerRegistry.PlayerTransform != null) player = PlayerRegistry.PlayerTransform;
//         if (PlayerRegistry.PlayerStats != null) ps = PlayerRegistry.PlayerStats;

//         if (player == null)
//         {
//             var go = GameObject.FindGameObjectWithTag("Player");
//             if (go != null) player = go.transform;
//         }
//         if (ps == null && player != null)
//             ps = player.GetComponent<PlayerStats>();

//         if (ps == null) ps = PlayerStats.Instance;
//     }

//     void OnScore(long v)
// {
//     // 10-digit score like 0000000123 (match your old TMP format)
//     if (scoreDigits) scoreDigits.SetNumber(v, minDigits: 7);
// }

//     void OnCombo(int c, int m)
//     {
//         if (comboDigits) comboDigits.SetNumber(c, minDigits: 2);
//         if (multiplierDigits) multiplierDigits.SetMultiplier(m); // shows x#
//     }

//     void OnSouls(int s)
//     {
//         if (soulsDigits) soulsDigits.SetNumber(s, minDigits: 3);
//     }

//     void OnKills(int s)
//     {
//         if (killsDigits) killsDigits.SetNumber(s, minDigits: 3);
//     }


// }

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelUI : MonoBehaviour
{
    [Header("Labels")]
    public TMP_Text stageText;
    public TMP_Text difficultyText;

    [Header("Handwritten Digits")]
    public DigitStripUI scoreDigits;
    public DigitStripUI comboDigits;
    public DigitStripUI multiplierDigits;
    public DigitStripUI soulsDigits;
    public DigitStripUI killsDigits;

    [Header("Player TMP Fallback (optional)")]
    public TMP_Text soulsText;
    public TMP_Text killsText;

    [Header("Lives/Bombs")]
    public TMP_Text livesCountText;
    public TMP_Text bombsCountText;
    public Image[] lifeIcons;
    public Image[] bombIcons;

    [SerializeField] private string stageLabel = "Stage 1";
    [SerializeField] private string difficultyLabel = "NORMAL";

    [SerializeField] private Transform digitsRoot; // drag MyDigitsUI here (or leave empty to auto-find)


    private PlayerStats ps;

    void Awake()
    {
        if (!digitsRoot)
        {
            // Try to find by name in children first
            var t = transform.Find("MyDigitsUI");
            digitsRoot = t ? t : transform;
        }

        if (!scoreDigits)      scoreDigits      = digitsRoot.Find("ScoreDigits")?.GetComponent<DigitStripUI>();
        if (!comboDigits)      comboDigits      = digitsRoot.Find("ComboDigits")?.GetComponent<DigitStripUI>();
        if (!multiplierDigits) multiplierDigits = digitsRoot.Find("MultiplierDigits")?.GetComponent<DigitStripUI>();
        if (!soulsDigits)      soulsDigits      = digitsRoot.Find("SoulDigits")?.GetComponent<DigitStripUI>();
        if (!killsDigits)      killsDigits      = digitsRoot.Find("KillsDigits")?.GetComponent<DigitStripUI>();
    }

    void OnEnable()
    {
        if (stageText) stageText.text = stageLabel;
        if (difficultyText) difficultyText.text = difficultyLabel;

        // Subscribe ScoreKeeper
        if (ScoreKeeper.Instance != null)
        {
            ScoreKeeper.Instance.OnScoreChanged += OnScore;
            ScoreKeeper.Instance.OnComboChanged += OnCombo;

            // push initial values
            OnScore(ScoreKeeper.Instance.Score);
            OnCombo(ScoreKeeper.Instance.Combo, ScoreKeeper.Instance.Multiplier);
        }
        else
        {
            Debug.LogWarning("RightPanelUI: ScoreKeeper.Instance is null in OnEnable()");
        }

        // Subscribe PlayerStats
        ps = PlayerStats.Instance;
        if (ps != null)
        {
            Debug.Log($"[UI] subscribed PlayerStats InstanceID={ps.GetInstanceID()}");

            ps.OnLivesChanged += OnLives;
            ps.OnBombsChanged += OnBombs;
            ps.OnSoulsChanged += OnSouls;
            ps.OnKillsChanged += OnKills;

            // push initial values
            OnLives(ps.lives);
            OnBombs(ps.bombs);
            OnSouls(ps.souls);
            OnKills(ps.kills);
        }
        else
        {
            Debug.LogWarning("RightPanelUI: PlayerStats.Instance is null in OnEnable() (will retry in Update)");
        }
    }

    void Update()
    {
        // If PlayerStats wasn't ready in OnEnable, keep trying
        if (ps == null)
        {
            ps = PlayerStats.Instance;
            if (ps != null)
            {
                Debug.Log($"[UI] late-bound PlayerStats InstanceID={ps.GetInstanceID()}");

                ps.OnLivesChanged += OnLives;
                ps.OnBombsChanged += OnBombs;
                ps.OnSoulsChanged += OnSouls;
                ps.OnKillsChanged += OnKills;

                OnLives(ps.lives);
                OnBombs(ps.bombs);
                OnSouls(ps.souls);
                OnKills(ps.kills);
            }
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
            ps.OnKillsChanged -= OnKills;
        }
    }

    // -----------------------------
    // UI update callbacks
    // -----------------------------

    void OnScore(long v)
    {
        Debug.Log($"RightPanelUI OnScore: {v}, scoreDigits={(scoreDigits ? scoreDigits.name : "NULL")}");
        Debug.LogWarning($"RightPanelUI({gameObject.scene.name}/{name}): multiplierDigits not assigned");


        // Example: 10 digits, right-aligned
        if (scoreDigits) scoreDigits.SetNumber(v, minDigits: 7);
        else Debug.LogWarning("RightPanelUI: scoreDigits not assigned");
    }

    void OnCombo(int c, int m)
    {
        if (comboDigits) comboDigits.SetNumber(c, minDigits: 1);
        else Debug.LogWarning("RightPanelUI: comboDigits not assigned");

        if (multiplierDigits) multiplierDigits.SetMultiplier(m); // shows x#
        else Debug.LogWarning("RightPanelUI: multiplierDigits not assigned");
    }

    void OnSouls(int s)
    {
        if (soulsDigits) soulsDigits.SetNumber(s, minDigits: 1);
        if (soulsText) soulsText.text = s.ToString(); // fallback TMP
    }

    void OnKills(int s)
    {
        if (killsDigits) killsDigits.SetNumber(s, minDigits: 1);
        if (killsText) killsText.text = s.ToString(); // fallback TMP
    }

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


}
