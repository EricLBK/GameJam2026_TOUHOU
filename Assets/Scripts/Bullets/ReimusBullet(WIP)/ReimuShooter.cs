using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    public class ReimuShooter : MonoBehaviour
    {
        [Header("Refs")]
        public BulletManager bulletManager;   // Drag your BulletManager here

        [Header("Fire Settings")]
        public KeyCode fireKey = KeyCode.Z;
        public float firePeriod = 0.12f;      // seconds between shots when holding key
        public float bulletSpeed = 350f;

        [Header("Shotgun Settings")]
        public float spreadDeg = 15f;         // angle between adjacent pellets
        public float bulletRadius = 50f;      // passed into SpawnBullet for collision logic

        [Header("Optional Spawn Offset")]
        public float muzzleForwardOffset = 0f; // set >0 if you want bullets to start slightly above Reimu

        private float _nextFireTime;

        void Update()
        {
            if (bulletManager == null) return;

            if (!Input.GetKey(fireKey)) return;
            if (Time.time < _nextFireTime) return;

            _nextFireTime = Time.time + firePeriod;

            // Reimu origin in world space
            Vector2 originV2 = transform.position;

            // Touhou-style base direction: straight up (90 degrees)
            float baseDeg = 90f;

            // Optional small forward offset (still centered on origin)
            Vector2 spawnV2 = originV2 + Vector2.up * muzzleForwardOffset;

            // 5 pellets: -2, -1, 0, +1, +2 (mirrored)
            for (int k = -2; k <= 2; k++)
            {
                float deg = baseDeg + (k * spreadDeg);
                Vector2 vel2 = Util.DegreeToVector2(deg) * bulletSpeed;

                bulletManager.SpawnBullet(
                    position: new float2(spawnV2.x, spawnV2.y),
                    velocity: (float2)vel2,
                    path: null,
                    radius: bulletRadius
                );
            }
        }
    }
}
