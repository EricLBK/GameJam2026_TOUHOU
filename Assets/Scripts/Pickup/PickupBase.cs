using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    [Header("Collect")]
    [Tooltip("Extra radius added to the player's pickupRadius (feel-good).")]
    [Min(0f)] public float pickupPadding = 0.0f;

    [Header("Magnet (optional)")]
    public bool useMagnet = true;
    [Min(0f)] public float magnetRadius = 80f;
    [Min(0f)] public float magnetSpeed = 500f;

    [Header("Fall (optional)")]
    public bool fallDown = true;
    [Min(0f)] public float fallSpeed = 100f;

    Transform player;
    PlayerStats stats;

    protected virtual void Start()
    {
        // Works without dragging anything in the Inspector
        player = PlayerRegistry.PlayerTransform != null ? PlayerRegistry.PlayerTransform : (PlayerStats.Instance?.transform);
        stats  = PlayerRegistry.PlayerStats != null ? PlayerRegistry.PlayerStats : PlayerStats.Instance;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (fallDown)
            transform.position += Vector3.down * (fallSpeed * dt);

        if (player == null || stats == null) return;

        Vector2 p = player.position;
        Vector2 me = transform.position;

        float dist = (p - me).magnitude;

        // Magnet
        if (useMagnet && dist <= magnetRadius && dist > 0.0001f)
        {
            Vector2 dir = (p - me).normalized;
            transform.position += (Vector3)(dir * magnetSpeed * dt);
        }

        // Collect (distance-based)
        float collectR = stats.pickupRadius + pickupPadding;
        if (dist <= collectR)
        {
            OnCollect(stats);
            Destroy(gameObject);
        }
    }

    protected abstract void OnCollect(PlayerStats stats);
}
