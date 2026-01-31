using UnityEngine;

namespace Bullets.Patterns
{
    public abstract class BulletPattern : ScriptableObject
    {
        public float bulletsPerSecond = 10f;
        public float bulletSpeed = 50f;
        public float bulletSize = 40f;
        
        public abstract void Execute(BulletManager manager, Transform emitter, float time);
    }
}