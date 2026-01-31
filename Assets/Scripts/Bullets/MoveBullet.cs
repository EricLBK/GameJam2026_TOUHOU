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
    
        // We still need this to know if it's active and what its velocity is
        public NativeArray<BulletData> Bullets; 

        // The "Execute" signature is different for Transform jobs
        public void Execute(int index, TransformAccess transform)
        {
            var b = Bullets[index];
            if (!b.IsActive) 
            {
                // Optional: Hide inactive bullets by moving them far away
                // Real pooling would disable the GameObject, but we can't do that in a Job.
                transform.position = new Vector3(1000, 1000, 0); 
                return;
            }

            // 1. Update Position Logic
            float3 currentPos = transform.position;
            currentPos.x += b.Velocity.x * DeltaTime;
            currentPos.y += b.Velocity.y * DeltaTime;

            // 2. Bounds Check
            // if (currentPos.x < BoundsMin.x || currentPos.x > BoundsMax.x ||
            //     currentPos.y < BoundsMin.y || currentPos.y > BoundsMax.y)
            // {
            //     b.IsActive = 0;
            // }
        
            // 3. Apply changes
            transform.position = currentPos;
        
            // 4. Update Rotation (Optional - align to velocity)
            if (math.lengthsq(b.Velocity) > 0.001f)
            {
                var angle = math.atan2(b.Velocity.y, b.Velocity.x);
                transform.rotation = quaternion.RotateZ(angle - math.PI / 2); // -90 deg offset for Up-facing sprites
            }

            // Write back state changes
            Bullets[index] = b;
        }
    }

    [BurstCompile]
    public struct CollisionJob : IJobParallelFor
    {
        public float2 PlayerPosition;
        public float PlayerRadiusSq; // squared radius
        [ReadOnly] public NativeArray<BulletData> Bullets;
        
        [NativeDisableParallelForRestriction]
        [WriteOnly] 
        public NativeReference<int> HitDetected;
        
        
        public void Execute(int index)
        {
            var bullet = Bullets[index];
            if (!bullet.IsActive)
            {
                return;
            }

            var distSq = math.distancesq(bullet.Position, PlayerPosition);
            var combinedRadius = bullet.Radius;
            if (distSq < (combinedRadius * combinedRadius) + PlayerRadiusSq)
            {
                HitDetected.Value = 1;
            }
        }
    }
}
