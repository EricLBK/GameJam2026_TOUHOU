using System;
using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    public class ReimuShooter : MonoBehaviour
    {
        [Header("Refs")]
        public BulletManager bulletManager;

        private BulletPrefab _prefab;

        [Header("Input")]
        public KeyCode fireKey = KeyCode.Z;

        [Tooltip("Hold either shift key to enter focus mode.")]
        public KeyCode focusKeyLeft = KeyCode.LeftShift;
        public KeyCode focusKeyRight = KeyCode.RightShift;

        [Header("Fire Rate")]
        public float firePeriodNormal = 0.12f;
        public float firePeriodFocus = 0.08f;

        [Header("Bullet")]
        public float bulletSpeed = 350f;
        public float bulletRadius = 50f;
        public float muzzleForwardOffset = 0f; // optional: spawn slightly above Reimu

        [Header("Normal Shotgun")]
        public float spreadDeg = 15f; // angle between adjacent pellets (normal mode)

        [Header("Homing (normal mode only, extremes only)")]
        public Transform target;               // null => no enemies => no homing
        public float turnRateDegPerSec = 240f;
        public float homingDelaySeconds = 0.35f;

        public bool disableShooting = false;

        [Header("Focus Mode (4 straight lanes)")]
        [Tooltip("Distance between adjacent lanes (world units).")]
        public float focusLaneSpacing = 30f;

        private float _nextFireTime;

        void Update()
        {
            if (bulletManager == null) return;

            bool firing = Input.GetKey(fireKey);
            if (!firing || disableShooting) return;

            bool focus = Input.GetKey(focusKeyLeft) || Input.GetKey(focusKeyRight);

            float period = focus ? firePeriodFocus : firePeriodNormal;
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + period;

            Vector2 originV2 = transform.position;
            Vector2 spawnV2 = originV2 + Vector2.up * muzzleForwardOffset;

            // World-forward (Touhou style)
            Vector2 forward = Vector2.up;
            Vector2 right = Vector2.right;

            if (focus)
            {
                FireFocus4(spawnV2, forward, right);
            }
            else
            {
                FireNormal5(spawnV2);
            }
        }

        private void FireFocus4(Vector2 spawnV2, Vector2 forward, Vector2 right)
        {
            float2 basePos = new float2(spawnV2.x, spawnV2.y);
            float2 vel = (float2)(forward * bulletSpeed);

            // 4 lanes, equidistant, mirrored:
            // offsets = -1.5d, -0.5d, +0.5d, +1.5d
            float d = focusLaneSpacing;
            float[] laneOffsets = { -1.5f * d, -0.5f * d, +0.5f * d, +1.5f * d };

            for (int idx = 0; idx < laneOffsets.Length; idx++)
            {
                Vector2 lanePos2 = spawnV2 + right * laneOffsets[idx];

                bulletManager.SpawnBullet(
                    position: new float2(lanePos2.x, lanePos2.y),
                    velocity: vel,
                    isPlayerBullet: true,
                    prefab: BulletPrefabs.Load().spell2
                );
            }
        }

        private void FireNormal5(Vector2 spawnV2)
        {
            // Always base spread forward (up). Homing bullets will steer later.
            float baseDeg = 90f;

            BulletPath homingPath = null;
            if (target != null)
                homingPath = Paths.Homing(target, turnRateDegPerSec, homingDelaySeconds);

            // Right->Left order: k=+2,+1,0,-1,-2 (your numbering 1..5)
            for (int k = 2; k >= -2; k--)
            {
                float deg = baseDeg + (k * spreadDeg);
                Vector2 vel2 = Util.DegreeToVector2(deg) * bulletSpeed;

                // Only extremes (far right k=+2 and far left k=-2) home; middle 3 stay straight.
                BulletPath pathForThisBullet =
                    (homingPath != null && (k == 2 || k == -2)) ? homingPath : null;

                bulletManager.SpawnBullet(
                    position: new float2(spawnV2.x, spawnV2.y),
                    velocity: (float2)vel2,
                    path: pathForThisBullet,
                    isPlayerBullet: true,
                    prefab: BulletPrefabs.Load().spell1
                );
            }
        }
    }
}
