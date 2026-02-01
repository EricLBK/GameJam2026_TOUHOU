using System;
using System.Collections.Generic;
using Bullets;
using Unity.Mathematics;
using UnityEngine;

namespace Enemy
{
    public class EnemyController : MonoBehaviour
    {
        // NEW: Active enemy registry (so BulletManager can iterate enemies)
        public static readonly List<EnemyController> ActiveEnemies = new List<EnemyController>();

        [Header("Stats")]
        [SerializeField] private float maxHP = 10f;
        [SerializeField] private float speed = 0.5f; // 0.5 means 2 seconds to finish path

        [Header("Rewards")]
        [Tooltip("Base points awarded when this enemy is killed by the player.")]
        [SerializeField] private int pointsOnKill = 100;

        [Tooltip("How many Souls to drop when killed by the player.")]
        [SerializeField] private int soulsToDrop = 1;
        
        private Transform player;
        private PlayerStats stats;

        private float _hitRadius;
        private float _currentHP;
        private float _pathProgress; // 't' value (0.0 to 1.0)
        private EnemyPath _currentPath;
        private Rect _cullingRect;
        private BulletManager _bulletManager;
        private Action<EnemyController> _onDeathCallback; // To return to pool

        private void OnEnable()
        {
            // Register when enabled (works with pooling)
            if (!ActiveEnemies.Contains(this))
                ActiveEnemies.Add(this);
        }

        private void OnDisable()
        {
            ActiveEnemies.Remove(this);
        }

        // Initialize is called by the Manager when spawning
        public void Initialize(
            EnemyPath path,
            BulletManager bulletManager,
            Rect cullRect,
            float hitRadius,
            Action<EnemyController> returnToPool
        )
        {
            _currentPath = path;
            _cullingRect = cullRect;
            _onDeathCallback = returnToPool;
            _bulletManager = bulletManager;

            _currentHP = maxHP;
            _pathProgress = 0f;
            _hitRadius = hitRadius;

            // Reset Visuals
            gameObject.SetActive(true);
            transform.position = _currentPath.points[0];
        }

        void Update()
        {
            MoveAlongPath();
            CheckBounds();
            if (stats == null)
            {
                ResolveRefs();
                return;
            }
        }

        private void MoveAlongPath()
        {
            if (_currentPath == null) return;

            // Advance 't' based on speed and time
            _pathProgress += speed * Time.deltaTime;

            // Calculate new position
            transform.position = _currentPath.Evaluate(_pathProgress);
        }

        private void CheckBounds()
        {
            // 1. Check if path is finished
            if (_pathProgress >= 1f)
            {
                Kill(false); // Despawn without rewards
                return;
            }

            if (!(_pathProgress % 0.25f <= 0.02f)) return;

            // bounds checks omitted (as in your code)
        }

        // Call this when a bullet hits the enemy
        public void TakeDamage(float damage)
        {
            _currentHP -= damage;

            if (_currentHP <= 0)
            {
                Kill(true); // Die with explosion / rewards
            }
        }

        private void Kill(bool explode)
        {
            if (explode)
            {
                // Instantiate Explosion Particle

                // --- Rewards (only when actually killed) ---
                if (pointsOnKill > 0 && ScoreKeeper.Instance != null)
                    ScoreKeeper.Instance.AddScore(pointsOnKill);
                    stats.AddKills(1);

                if (soulsToDrop > 0 && ItemManager.Instance != null)
                {
                    Vector2 p = transform.position;
                    if (soulsToDrop == 1) ItemManager.Instance.SpawnSoul(p);
                    else ItemManager.Instance.SpawnSoulBurst(p, soulsToDrop);
                }
            }

            // Return to pool
            gameObject.SetActive(false);
            _onDeathCallback?.Invoke(this);
        }

        // -------------------------
        // NEW: AABB Hurtbox Helpers
        // -------------------------

        public void GetHurtboxAabb(out float2 min, out float2 max)
        {
            // Use a square box of size 2*hitRadius (simple + adjustable via hitRadius)
            Vector3 p = transform.position;
            float2 c = new float2(p.x, p.y);
            float2 h = new float2(_hitRadius, _hitRadius);

            min = c - h;
            max = c + h;
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
    }
}
