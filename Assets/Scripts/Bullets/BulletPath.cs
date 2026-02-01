using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Bullets
{
    // CHANGED: added getPosition
    public delegate IEnumerable BulletPath(
        Vector2 startVelocity,
        Action<Vector2> setVelocity,
        Func<Vector2> getPosition
    );

    public static class Paths
    {
        // CHANGED: signature now includes getPosition (unused here)
        public static BulletPath Sine(float amplitude, float frequency)
        {
            IEnumerable path(
                Vector2 startVelocity,
                Action<Vector2> setVelocity,
                Func<Vector2> getPosition
            )
            {
                var start = Time.time;
                for (; ; )
                {
                    var t = Time.time - start;
                    var angleDiff = amplitude * math.sin(t * frequency * math.PI2);
                    setVelocity(math.mul(float2x2.Rotate(angleDiff), (float2)startVelocity));
                    yield return new WaitForEndOfFrame();
                }
            }
            return path;
        }

        // NEW: homing path
        public static BulletPath Homing(
            Transform target,
            float turnRateDegPerSec,
            float delaySeconds = 0f
        )
        {
            IEnumerable path(
                Vector2 startVelocity,
                Action<Vector2> setVelocity,
                Func<Vector2> getPosition
            )
            {
                float speed = startVelocity.magnitude;
                if (speed < 1e-4f)
                    speed = 1f;

                Vector2 currentVel = startVelocity;
                float maxTurnRadPerSec = turnRateDegPerSec * Mathf.Deg2Rad;

                float startTime = Time.time;

                for (; ; )
                {
                    // Delay phase: keep initial straight velocity (nice arc setup)
                    if (Time.time - startTime < delaySeconds)
                    {
                        setVelocity(currentVel);
                        yield return null;
                        continue;
                    }

                    Vector2 bulletPos = getPosition();
                    Vector2 toTarget = (Vector2)target.position - bulletPos;

                    Vector2 desiredDir =
                        toTarget.sqrMagnitude < 1e-6f ? currentVel.normalized : toTarget.normalized;

                    Vector2 currentDir =
                        currentVel.sqrMagnitude < 1e-6f ? desiredDir : currentVel.normalized;

                    float cross = currentDir.x * desiredDir.y - currentDir.y * desiredDir.x;
                    float dot = Mathf.Clamp(Vector2.Dot(currentDir, desiredDir), -1f, 1f);
                    float angle = Mathf.Atan2(cross, dot);

                    float maxStep = maxTurnRadPerSec * Time.deltaTime;
                    float clamped = Mathf.Clamp(angle, -maxStep, +maxStep);

                    float s = Mathf.Sin(clamped);
                    float c = Mathf.Cos(clamped);
                    Vector2 newDir = new Vector2(
                        c * currentDir.x - s * currentDir.y,
                        s * currentDir.x + c * currentDir.y
                    );

                    currentVel = newDir * speed;
                    setVelocity(currentVel);

                    yield return null;
                }
            }
            return path;
        }
    }
}
