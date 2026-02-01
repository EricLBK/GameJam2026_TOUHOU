// ScoreUI.cs
// Attach to a UI GameObject. Link TextMeshProUGUI fields.
// Requires TMPro package.

using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text multText;

    private void OnEnable()
    {
        if (ScoreKeeper.Instance == null) return;
        ScoreKeeper.Instance.OnScoreChanged += HandleScore;
        ScoreKeeper.Instance.OnComboChanged += HandleCombo;

        HandleScore(ScoreKeeper.Instance.Score);
        HandleCombo(ScoreKeeper.Instance.Combo, ScoreKeeper.Instance.Multiplier);
    }

    private void OnDisable()
    {
        if (ScoreKeeper.Instance == null) return;
        ScoreKeeper.Instance.OnScoreChanged -= HandleScore;
        ScoreKeeper.Instance.OnComboChanged -= HandleCombo;
    }

    private void HandleScore(long s)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE {s:n0}";
    }

    private void HandleCombo(int combo, int mult)
    {
        if (comboText != null)
            comboText.text = $"COMBO {combo}";
        if (multText != null)
            multText.text = $"x{mult}";
    }
}
