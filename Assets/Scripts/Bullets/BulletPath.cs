using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    public delegate IEnumerable BulletPath(Vector2 startVelocity, System.Action<Vector2> setVelocity);

    public static class Paths
    {
        public static BulletPath Sine(float amplitude, float frequency)
        {
            IEnumerable path(Vector2 startVelocity, System.Action<Vector2> setVelocity)
            {
                var start = Time.time;
                var startAngle = Vector2.Angle(startVelocity, Vector2.right);
                for (; ; )
                {
                    var t = Time.time - start;
                    var angleDiff = amplitude * math.sin(t * frequency * math.PI2);
                    setVelocity(math.mul(float2x2.Rotate(angleDiff), startVelocity));
                    yield return new WaitForEndOfFrame();
                }
            }
            return path;
        }
    }
}
