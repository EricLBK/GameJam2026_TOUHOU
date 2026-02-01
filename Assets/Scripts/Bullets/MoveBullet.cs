using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace Bullets
{

    [BurstCompile]
    public struct MoveBulletJob : IJobParallelForTransform
    {
        public float DeltaTime;
        public float2 BoundsMin;
        public float2 BoundsMax;

        public static readonly Vector3 FAR_AWAY = new(1000, 1000, 0);

        [ReadOnly]
        public NativeArray<float2> Velocity;
        public NativeArray<float2> Position;
        public NativeArray<bool> IsActive;

        // The "Execute" signature is different for Transform jobs
        public void Execute(int index, TransformAccess transform)
        {
            if (!IsActive[index])
            {
                transform.position = FAR_AWAY;
                return;
            }

            // 1. Update Position Logic
            var v = Velocity[index];
            var p = Position[index] + v * DeltaTime;

            // 2. Bounds Check
            if (p.x < BoundsMin.x || p.x > BoundsMax.x ||
                p.y < BoundsMin.y || p.y > BoundsMax.y)
            {
                IsActive[index] = false;
                transform.position = FAR_AWAY;
            }
            else
            {
                // 3. Apply changes
                transform.position = new Vector3(p.x, p.y);

                // 4. Update Rotation (Optional - align to velocity)
                if (math.lengthsq(v) > 0.001f)
                {
                    var angle = math.atan2(v.y, v.x);
                    transform.rotation = quaternion.RotateZ(angle - math.PI / 2); // -90 deg offset for Up-facing sprites
                }
            }

            // Write back state changes
            Position[index] = p;
        }
    }

    [BurstCompile]
    public struct CollisionJob : IJobParallelFor
    {
        public float2 PlayerPosition;
        public float PlayerRadiusSq; // squared radius
        public BulletData Bullets;

        [NativeDisableParallelForRestriction]
        [WriteOnly]
        public NativeReference<int> HitDetected;


        public void Execute(int index)
        {
            if (!Bullets.IsActive[index] || Bullets.IsPlayerBullet[index])
            {
                return;
            }

            var distSq = math.distancesq(Bullets.Position[index], PlayerPosition);
            var combinedRadius = Bullets.Radius[index];
            // Debug.Log($"bullet pos: {bullet.Position}, player pos: {PlayerPosition}");

            if (!(distSq < (combinedRadius * combinedRadius) + PlayerRadiusSq)) return;

            HitDetected.Value = 1;
            Bullets.IsActive[index] = false;
        }
    }
}
