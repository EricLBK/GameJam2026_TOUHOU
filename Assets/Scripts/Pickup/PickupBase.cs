using UnityEngine;

public abstract class PickupBase : MonoBehaviour
{
    [Header("Collect")]
    [Tooltip("Extra radius added to the player's pickupRadius (feel-good).")]
    [SerializeField] public float pickupPadding = 5f;

    [Header("Magnet (optional)")]
    public bool useMagnet = true;
    [SerializeField] public float magnetRadius = 80f;
    [SerializeField] public float magnetSpeed = 500f;

    [Header("Fall (optional)")]
    public bool fallDown = true;
    [SerializeField] public float fallSpeed = 100f;

    Transform player;
    PlayerStats stats;

    void OnEnable()
    {
        ResolveRefs(); // important for pooled objects too
    }

    protected virtual void Start()
    {
        ResolveRefs();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Always fall (independent from whether player refs exist)
        if (fallDown)
            transform.position += Vector3.down * (fallSpeed * dt);

        // If refs missing, keep trying to resolve them
        if (player == null || stats == null)
        {
            ResolveRefs();
            return;
        }

        Vector2 p = player.position;
        Vector2 me = transform.position;

        float dist = (p - me).magnitude;

        // Magnet
        if (useMagnet && dist <= magnetRadius && dist > 0.0001f)
        {
            Debug.Log($"Magneting to: {player.name} at {player.position}");

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

    void ResolveRefs()
    {
        // Prefer registry if available
        if (PlayerRegistry.PlayerTransform != null) player = PlayerRegistry.PlayerTransform;
        if (PlayerRegistry.PlayerStats != null) stats = PlayerRegistry.PlayerStats;

        // Fallback to singleton if registry missing/not ready
        if (player == null && PlayerStats.Instance != null) player = PlayerStats.Instance.transform;
        if (stats == null) stats = PlayerStats.Instance;
    }

    protected abstract void OnCollect(PlayerStats stats);
}
