using UnityEngine;

public class SoulPickup : MonoBehaviour
{
    [SerializeField] public int value = 50;

    [SerializeField] private float fallSpeed = 100f;
    [SerializeField] private float magnetRadius = 80f;
    [SerializeField] private float magnetSpeed = 500f;
    [SerializeField] private float collectRadius = 40f;

    Transform player;
    float playerPickupRadius;

    public void Init(Transform playerTransform, float pickupRadius)
    {
        player = playerTransform;
        playerPickupRadius = pickupRadius;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (player == null)
        {
            transform.position += Vector3.down * (fallSpeed * dt);
            return;
        }

        Vector2 soulPos = transform.position;
        Vector2 playerPos = player.position;
        float dist = (playerPos - soulPos).magnitude;

        if (dist <= magnetRadius)
        {
            Vector2 dir = (playerPos - soulPos).normalized;
            transform.position += (Vector3)(dir * magnetSpeed * dt);
        }
        else
        {
            transform.position += Vector3.down * (fallSpeed * dt);
        }

        if (dist <= collectRadius + playerPickupRadius)
        {
            Collect();
        }
    }


    void Collect()
    {
        if (ScoreKeeper.Instance != null)
            ScoreKeeper.Instance.AddPickup(value);

        SoulManager.Instance.Despawn(this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }

}
