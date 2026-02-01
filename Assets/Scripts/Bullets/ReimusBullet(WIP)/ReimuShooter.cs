using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    public class ReimuShooter : MonoBehaviour
    {
        [Header("Refs")]
        public BulletManager bulletManager;

        [Header("Input")]
        public KeyCode fireKey = KeyCode.Z;
        public float firePeriod = 0.12f;

        [Header("Bullet")]
        public float bulletSpeed = 350f;
        public float bulletRadius = 50f;

        [Header("Shotgun")]
        public float spreadDeg = 15f;          // angle between adjacent pellets
        public float muzzleForwardOffset = 0f; // optional: push spawn slightly upward

        [Header("Homing (only if target exists)")]
        public Transform target;               // null => no enemies => straight shots
        public float turnRateDegPerSec = 240f;

        private float _nextFireTime;

        void Update()
        {
            if (bulletManager == null) return;

            if (!Input.GetKey(fireKey)) return;
            if (Time.time < _nextFireTime) return;

            _nextFireTime = Time.time + firePeriod;

            Vector2 originV2 = transform.position;
            Vector2 spawnV2 = originV2 + Vector2.up * muzzleForwardOffset;

            // Always shoot "forward" (straight up) for base spread.
            // Homing bullets will steer after spawn if target exists.
            float baseDeg = 90f;

            BulletPath homingPath = null;
            if (target != null)
                homingPath = Paths.Homing(target, turnRateDegPerSec);

            // k values map to pellets:
            // k=+2 => far right  (pellet #1)  HOMING
            // k=+1 => right      (pellet #2)  STRAIGHT
            // k= 0 => center     (pellet #3)  STRAIGHT
            // k=-1 => left       (pellet #4)  STRAIGHT
            // k=-2 => far left   (pellet #5)  HOMING
            for (int k = 2; k >= -2; k--) // iterate right -> left to match your numbering
            {
                float deg = baseDeg + (k * spreadDeg);
                Vector2 vel2 = Util.DegreeToVector2(deg) * bulletSpeed;

                // Only extremes home; middle three always straight
                BulletPath pathForThisBullet =
                    (homingPath != null && (k == 2 || k == -2)) ? homingPath : null;

                bulletManager.SpawnBullet(
                    position: new float2(spawnV2.x, spawnV2.y),
                    velocity: (float2)vel2,
                    path: pathForThisBullet,
                    radius: bulletRadius
                );
            }
        }
    }
}
