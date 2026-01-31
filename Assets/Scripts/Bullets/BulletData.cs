using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace Bullets
{
    [System.Serializable]
    public struct BulletData
    {
        public NativeArray<float2> Position;
        public NativeArray<float2> Velocity;
        public NativeArray<float> Radius;
        public NativeArray<bool> IsActive;

        public BulletData(int size)
        {
            Position = new(size, Allocator.Persistent);
            Velocity = new(size, Allocator.Persistent);
            Radius = new(size, Allocator.Persistent);
            IsActive = new(size, Allocator.Persistent);
        }

        public void Dispose()
        {
            Position.Dispose();
            Velocity.Dispose();
            Radius.Dispose();
            IsActive.Dispose();
        }
    }
}
