using System.Collections;
using System.Collections.Generic;
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

        public static Vector2 RadToVector2(float rad)
        {
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        }
    }

    public delegate IEnumerator BulletPattern(BulletManager manager, float2 initPosition);

    public struct BulletSpawn
    {
        public float2 position;
        public float2 velocity;
    }

    public delegate List<BulletSpawn> BulletShot(float2 initPos);

    public class Shots
    {
        public static BulletShot Spread(
            float2 targetPos,
            float bulletSpeed = 300f,
            float spreadDegrees = 8f,
            int spreadCount = 2
        )
        {
            return (initPos) =>
            {
                float2 aimDir = math.normalize(targetPos - initPos);
                float baseDeg = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

                List<BulletSpawn> spawns = new();

                // mirrored around center
                for (int k = -spreadCount; k <= spreadCount; k++)
                {
                    float deg = baseDeg + (k * spreadDegrees);
                    float2 vel = Util.DegreeToVector2(deg) * bulletSpeed;
                    spawns.Add(new BulletSpawn { position = initPos, velocity = vel });
                }

                return spawns;
            };
        }

        public static BulletShot Wide(
            float2 targetPos,
            float bulletSpeed,
            float distanceBetweenBullets,
            int numBullets
        )
        {
            return (initPos) =>
            {
                float2 velocity = math.normalize(targetPos - initPos) * bulletSpeed;
                List<BulletSpawn> spawns = new();

                for (int i = -numBullets + 1; i <= numBullets - 1; i += 2)
                {
                    // rotate by 90 degrees
                    float2 offset =
                        math.normalize(new float2(velocity.y, -velocity.x))
                        * ((float)i / (numBullets - 1))
                        * distanceBetweenBullets;
                    spawns.Add(
                        new BulletSpawn { position = initPos + offset, velocity = velocity }
                    );
                }

                return spawns;
            };
        }
    }

    public class Patterns
    {
        public static BulletPattern Spiral(
            int numberOfArms = 4, // How many "spokes" the spiral has
            float rotationSpeed = 90f, // Degrees per second
            float bulletSpeed = 50f,
            bool spinClockwise = true,
            BulletPath path = null,
            BulletPrefab prefab = null
        )
        {
            IEnumerator execute(BulletManager manager, float2 initPosition)
            {
                var startTime = Time.time;

                // Angle step for multiple arms (e.g., 3 arms = 120 degrees apart)
                var angleStep = 360f / numberOfArms;

                for (; ; )
                {
                    // Calculate the base angle based on time
                    // (time * speed) makes it rotate over time
                    var currentRotation = Time.time * rotationSpeed * (spinClockwise ? -1f : 1f);

                    for (var i = 0; i < numberOfArms; i++)
                    {
                        var finalAngle = currentRotation + (angleStep * i);

                        // Convert Angle to Velocity Vector
                        var velocity = Util.DegreeToVector2(finalAngle) * bulletSpeed;

                        manager.SpawnBullet(initPosition, velocity, path: path, prefab: prefab);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
            }
            return execute;
        }

        public static BulletPattern ThrowCircle(
            float radius,
            int count,
            float2 velocity,
            float startAngle = 0f,
            BulletPrefab prefab = null
        )
        {
            IEnumerator execute(BulletManager manager, float2 initPosition)
            {
                for (int i = 0; i < count; ++i)
                {
                    float angle = startAngle + i * (math.PI2 / count);
                    float2 position =
                        initPosition + new float2(math.cos(angle), math.sin(angle)) * radius;
                    manager.SpawnBullet(position: position, velocity: velocity, prefab: prefab);
                }
                yield break;
            }
            return execute;
        }

        public static BulletPattern SingleShot(
            BulletShot shot,
            BulletPath path = null,
            BulletPrefab prefab = null
        )
        {
            IEnumerator execute(BulletManager manager, float2 initPosition)
            {
                manager.SpawnShot(initPosition, shot, path: path, prefab: prefab);
                yield break;
            }

            return execute;
        }

        public static BulletPattern Shotgun(
            Transform target,
            float bulletSpeed = 300f,
            float spreadDegrees = 8f,
            float firePeriod = 0.5f,
            BulletPath path = null,
            BulletPrefab prefab = null
        )
        {
            IEnumerator execute(BulletManager manager, float2 initPosition)
            {
                for (; ; )
                {
                    manager.SpawnShot(
                        initPosition,
                        Shots.Spread(
                            (float2)(Vector2)target.position,
                            bulletSpeed: bulletSpeed,
                            spreadDegrees: spreadDegrees,
                            spreadCount: 2
                        ),
                        path: path,
                        prefab: prefab
                    );
                    yield return new WaitForSeconds(firePeriod);
                }
            }

            return execute;
        }

        public static BulletPattern Walls(
            int numberOfWalls,
            int armsPerWall,
            int armsPerGap,
            float baseAngle = 0, // TODO: not sure what this should be
            float bulletSpeed = 50f,
            float firePeriod = 0.5f,
            BulletPrefab prefab = null
        )
        {
            IEnumerator execute(BulletManager manager, float2 initPosition)
            {
                int armsPerSector = armsPerWall + armsPerGap;
                int totalArms = armsPerSector * numberOfWalls;

                for (; ; )
                {
                    for (int i = 0; i < numberOfWalls; ++i)
                    {
                        for (int j = 0; j < armsPerWall; ++j)
                        {
                            float angle =
                                (baseAngle + ((i * armsPerSector) + j) / (float)totalArms)
                                * math.PI2;
                            Vector2 vel = Util.RadToVector2(angle) * bulletSpeed;

                            manager.SpawnBullet(
                                position: initPosition,
                                velocity: vel,
                                prefab: prefab
                            );
                        }
                    }
                    yield return new WaitForSeconds(firePeriod);

                    for (int i = 0; i < numberOfWalls; ++i)
                    {
                        for (int j = armsPerGap; j < armsPerSector; ++j)
                        {
                            float angle =
                                (
                                    baseAngle
                                    + ((2 * (i * armsPerSector + j) + 1) / (float)(2 * totalArms))
                                ) * math.PI2;
                            Vector2 vel = Util.RadToVector2(angle) * bulletSpeed;

                            manager.SpawnBullet(
                                position: initPosition,
                                velocity: vel,
                                prefab: prefab
                            );
                        }
                    }
                    yield return new WaitForSeconds(firePeriod);
                }
            }

            return execute;
        }
    }
}
