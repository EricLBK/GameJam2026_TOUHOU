using UnityEngine;

public class ExtraLifePickup : PickupBase
{
    public int livesToAdd = 1;

    protected override void OnCollect(PlayerStats stats)
    {
        stats.AddLife(livesToAdd);
    }
}
