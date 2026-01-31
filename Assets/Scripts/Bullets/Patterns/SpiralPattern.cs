using UnityEngine;

namespace Bullets.Patterns
{
    public class SpiralPattern : BulletPattern
    {
        public int numberOfArms = 4; // How many "spokes" the spiral has
        public float rotationSpeed = 90f; // Degrees per second
        public bool spinClockwise = true;

        public override void Execute(BulletManager manager, Transform emitter, float time)
        {
            // Calculate the base angle based on time
            // (time * speed) makes it rotate over time
            var currentRotation = (time * rotationSpeed) * (spinClockwise ? -1f : 1f);

            // Angle step for multiple arms (e.g., 3 arms = 120 degrees apart)
            var angleStep = 360f / numberOfArms;

            for (var i = 0; i < numberOfArms; i++)
            {
                var finalAngle = currentRotation + (angleStep * i);
            
                // Convert Angle to Velocity Vector
                var velocity = DegreeToVector2(finalAngle) * (bulletSpeed + 60);

                manager.SpawnBullet(emitter.position, velocity, bulletSize);
            }
        }

        // Helper: Math magic to turn an angle into a Vector
        private Vector2 DegreeToVector2(float degree)
        {
            var radian = degree * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }
    }
}