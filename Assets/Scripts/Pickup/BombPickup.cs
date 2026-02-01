using UnityEngine;

public class BombPickup : PickupBase
{
    public int bombsToAdd = 1;

    protected override void OnCollect(PlayerStats stats)
    {
        stats.AddBomb(bombsToAdd);
    }
}
