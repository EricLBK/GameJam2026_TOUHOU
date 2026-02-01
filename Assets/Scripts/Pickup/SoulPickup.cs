using UnityEngine;

public class SoulPickup : PickupBase
{
    [Header("Rewards")]
    public int soulAmount = 1;
    public int scoreValue = 50;

    protected override void OnCollect(PlayerStats stats)
    {
        stats.AddSouls(soulAmount);

        if (ScoreKeeper.Instance != null)
            ScoreKeeper.Instance.AddScore(scoreValue);
    }
}
