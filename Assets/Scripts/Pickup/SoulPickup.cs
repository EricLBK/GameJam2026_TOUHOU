using UnityEngine;

public class SoulPickup : PickupBase
{
    [Header("Soul")]
    public int soulAmount = 1;
    public int scoreAmount = 50;

    protected override void OnCollect(PlayerStats stats)
    {
        stats.AddSouls(soulAmount);
        stats.AddScore(scoreAmount);
    }
}
