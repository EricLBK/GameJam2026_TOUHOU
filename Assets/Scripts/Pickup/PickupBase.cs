using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    [Header("Collect")]
    [Min(0f)] public float pickupPadding = 0.0f;

    [Header("Magnet")]
    public bool useMagnet = true;
    [Min(0f)] public float magnetRadius = 3f;
    [Min(0f)] public float magnetSpeed = 10f;

    [Header("Fall")]
    public bool fallDown = true;
    [Min(0f)] public float fallSpeed = 2.5f;

    Transform player;
    PlayerStats stats;

    protected virtual void OnEnable() => ResolveRefs();
    protected virtual void Start() => ResolveRefs();

    void Update()
    {
        float dt = Time.deltaTime;

        if (fallDown)
            transform.position += Vector3.down * (fallSpeed * dt);

        if (player == null || stats == null)
        {
            ResolveRefs();
            return;
        }

        Vector2 p = player.position;
        Vector2 me = transform.position;

        Vector2 delta = p - me;
        float distSq = delta.sqrMagnitude;

        // Magnet
        if (useMagnet && distSq <= magnetRadius * magnetRadius && distSq > 0.000001f)
        {
            transform.position += (Vector3)(delta.normalized * magnetSpeed * dt);
        }

        // Collect
        float collectR = stats.pickupRadius + pickupPadding;
        if (distSq <= collectR * collectR)
        {
            OnCollect(stats);
            Destroy(gameObject);
        }
    }

    void ResolveRefs()
    {
        if (PlayerRegistry.PlayerTransform != null) player = PlayerRegistry.PlayerTransform;
        if (PlayerRegistry.PlayerStats != null) stats = PlayerRegistry.PlayerStats;

        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) player = go.transform;
        }
        if (stats == null && player != null)
            stats = player.GetComponent<PlayerStats>();

        if (stats == null) stats = PlayerStats.Instance;
    }

    protected abstract void OnCollect(PlayerStats stats);
}
