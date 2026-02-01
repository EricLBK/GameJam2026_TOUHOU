using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    public class Util
    {
        public static Vector2 DegreeToVector2(float degree)
        {
            var radian = degree * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }

    public delegate IEnumerator BulletPattern(BulletManager manager);

    public class Patterns
    {
        public static BulletPattern Spiral(
            float2 position,
            int numberOfArms = 4, // How many "spokes" the spiral has
            float rotationSpeed = 90f, // Degrees per second
            float bulletSpeed = 50f,
            bool spinClockwise = true,
            float duration = 10.0f,
            BulletPath path = null
        )
        {
            IEnumerator execute(BulletManager manager)
            {
                var startTime = Time.time;
                var endTime = startTime + duration;

                // Angle step for multiple arms (e.g., 3 arms = 120 degrees apart)
                var angleStep = 360f / numberOfArms;

                while (Time.time < endTime)
                {
                    // Calculate the base angle based on time
                    // (time * speed) makes it rotate over time
                    var currentRotation = Time.time * rotationSpeed * (spinClockwise ? -1f : 1f);

                    for (var i = 0; i < numberOfArms; i++)
                    {
                        var finalAngle = currentRotation + (angleStep * i);

                        // Convert Angle to Velocity Vector
                        var velocity = Util.DegreeToVector2(finalAngle) * bulletSpeed;

                        manager.SpawnBullet(position, velocity, radius: 50, path: path);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
            }
            return execute;
        }

        public static BulletPattern ThrowCircle(
            float radius,
            int count,
            float2 center,
            float2 velocity,
            float startAngle = 0f
        )
        {
            IEnumerator execute(BulletManager manager)
            {
                for (int i = 0; i < count; ++i)
                {
                    float angle = startAngle + i * (math.PI2 / count);
                    float2 position =
                        center + new float2(math.cos(angle), math.sin(angle)) * radius;
                    manager.SpawnBullet(position: position, velocity: velocity);
                }
                yield break;
            }
            return execute;
        }
    }
}
