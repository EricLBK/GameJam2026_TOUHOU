using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Bullets
{
    [BurstCompile]
    public struct MoveBulletJob : IJobParallelFor
    {
        public float DeltaTime;
        public float2 BoundsMin;
        public float2 BoundsMax;
        
        public NativeArray<BulletData> Bullets;
        
        public void Execute(int index)
        {
            var b = Bullets[index];

            if (b.IsActive == 0) return;

            b.Position += b.Velocity * DeltaTime;

            if (b.Position.x < BoundsMin.x || b.Position.x > BoundsMax.x ||
                b.Position.y < BoundsMin.y || b.Position.y > BoundsMax.y)
            {
                b.IsActive = 0;
            }

            // 5. Write back to array
            Bullets[index] = b;
        }
    }

    public struct CollisionJob : IJobParallelFor
    {
        public float2 PlayerPosition;
        public float PlayerRadiusSq; // squared radius
        [ReadOnly] public NativeArray<BulletData> Bullets;
        [WriteOnly] public NativeReference<int> HitDetected;
        
        
        public void Execute(int index)
        {
            var bullet = Bullets[index];
            if (bullet.IsActive == 0)
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