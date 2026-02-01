using UnityEngine;
using Enemy; // <-- IMPORTANT: your actual enemy namespace

namespace Bullets
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerBullet : MonoBehaviour
    {
        public int damage = 1;
        public float lifetime = 2.5f;

        private Rigidbody2D _rb;
        private float _deathTime;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.bodyType = RigidbodyType2D.Kinematic;

            var col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        void OnEnable()
        {
            _deathTime = Time.time + lifetime;
        }

        void Update()
        {
            if (Time.time >= _deathTime)
                Destroy(gameObject);
        }

        public void Init(Vector2 velocity, int dmg, float life)
        {
            damage = dmg;
            lifetime = life;
            _deathTime = Time.time + lifetime;
            _rb.linearVelocity = velocity;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // 🔑 THIS is the only enemy lookup that matters
            EnemyController enemy = other.GetComponentInParent<EnemyController>();
            if (enemy == null) return;

            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
