using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    [BurstCompile]
    public struct UpdateMatricesJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<BulletData> Bullets;
        [WriteOnly] public NativeArray<Matrix4x4> Matrices;
        public void Execute(int index)
        {
            var b = Bullets[index];
            if (b.IsActive == 0) { Matrices[index] = Matrix4x4.zero; return; }
            
            var angle = math.atan2(b.Velocity.y, b.Velocity.x);
            var rot = quaternion.RotateZ(angle);
            // If the Sprite points UP, we have to -90 degrees (pi/2) from angle
            Matrices[index] = Matrix4x4.TRS(
                new float3(b.Position.x, b.Position.y, 0),
                rot, 
                new float3(1f, 1f, 1f) // Scale 1 = 1 unit size. Adjust as needed.
            );
        }
    }
}