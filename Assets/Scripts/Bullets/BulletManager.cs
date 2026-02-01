using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs; // REQUIRED
using Enemy; // NEW

namespace Bullets
{
    public class BulletManager : MonoBehaviour
    {
        public Rect bounds = new(-1000f, -1000f, 2000f, 2000f);
        public int maxBullets = 2000;
        public GameObject bulletPrefab;

        public Transform playerTransform;
        public float playerHitBoxRadius = 10;
        public float bulletRadius = 50;

        // NEW: how much damage player bullets deal to enemies
        public float playerBulletDamage = 1f;

        // Data
        private BulletData _bulletData;
        private TransformAccessArray _transformAccessArray; // Special array for Transforms
        private List<GameObject> _bulletPool; // Keep track of GOs to destroy later
        private NativeReference<int> _playerHitFlag;
        private int _nextBulletIndex = 0;
        private Vector3 _spriteSize;

        private JobHandle _moveHandle;
        private JobHandle _collisionHandle;

        private void Start()
        {
            _bulletData = new BulletData(maxBullets);
            _playerHitFlag = new NativeReference<int>(Allocator.Persistent);
            _bulletPool = new List<GameObject>(maxBullets);

            // 1. Pre-instantiate all GameObjects (Object Pooling)
            var transforms = new Transform[maxBullets];
            for (var i = 0; i < maxBullets; i++)
            {
                var go = Instantiate(bulletPrefab, transform);
                var sr = bulletPrefab.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    _spriteSize = sr.sprite.bounds.size;
                    var newScale = new Vector3(
                        bulletRadius * 2 / _spriteSize.x,
                        bulletRadius * 2 / _spriteSize.y,
                        1f
                    );
                    go.transform.localScale = newScale;
                }
                go.transform.position = MoveBulletJob.FAR_AWAY;
                go.SetActive(true);
                _bulletPool.Add(go);
                transforms[i] = go.transform;
            }

            // 2. Create the TransformAccessArray
            _transformAccessArray = new TransformAccessArray(transforms);
        }

        private void Update()
        {
            var moveJob = new MoveBulletJob
            {
                DeltaTime = Time.deltaTime,
                BoundsMin = new float2(bounds.xMin, bounds.yMin),
                BoundsMax = new float2(bounds.xMax, bounds.yMax),
                Velocity = _bulletData.Velocity,
                Position = _bulletData.Position,
                IsActive = _bulletData.IsActive,
            };

            _moveHandle = moveJob.Schedule(_transformAccessArray);

            var playerPos = new float2(playerTransform.position.x, playerTransform.position.y);

            var collisionJob = new CollisionJob
            {
                PlayerPosition = playerPos,
                PlayerRadiusSq = playerHitBoxRadius * playerHitBoxRadius,
                Bullets = _bulletData,
                HitDetected = _playerHitFlag,
            };

            _collisionHandle = collisionJob.Schedule(maxBullets, 64, _moveHandle);

            // You complete here already, so it is SAFE to read/write bullet arrays after this line.
            _collisionHandle.Complete();

            // NEW: resolve player bullets hitting enemies (own physics, AABB)
            HandleEnemyHits();
        }

        void LateUpdate()
        {
            if (_playerHitFlag.Value == 1) { }
        }

        private void OnDestroy()
        {
            _collisionHandle.Complete();
            _bulletData.Dispose();
            if (_playerHitFlag.IsCreated)
                _playerHitFlag.Dispose();
            if (_transformAccessArray.isCreated)
                _transformAccessArray.Dispose();
        }

        private int FindNextSlot()
        {
            for (int i = _nextBulletIndex; i < maxBullets; ++i)
            {
                if (!_bulletData.IsActive[i])
                {
                    _nextBulletIndex = i + 1;
                    return i;
                }
            }
            for (int i = 0; i < _nextBulletIndex; ++i)
            {
                if (!_bulletData.IsActive[i])
                {
                    _nextBulletIndex = i + 1;
                    return i;
                }
            }
            return -1;
        }

        // CHANGED: added isPlayerBullet flag (defaults false)
        public void SpawnBullet(
            float2 position,
            float2 velocity,
            BulletPath path = null,
            float radius = 50.0f,
            bool isPlayerBullet = false
        )
        {
            _collisionHandle.Complete();
            _moveHandle.Complete();

            int i = FindNextSlot();
            if (i == -1)
            {
                return;
            }

            _bulletData.IsActive[i] = true;
            _bulletData.Position[i] = position;
            _bulletData.Velocity[i] = velocity;
            _bulletData.Radius[i] = radius;

            // NEW: tag ownership so only player bullets hurt enemies
            _bulletData.IsPlayerBullet[i] = isPlayerBullet;

            if (path == null)
            {
                return;
            }

            void setVelocity(Vector2 v)
            {
                _bulletData.Velocity[i] = (float2)v;
            }

            Vector2 getPosition()
            {
                float2 p = _bulletData.Position[i];
                return new Vector2(p.x, p.y);
            }

            IEnumerator moveBulletWhileActive()
            {
                foreach (var step in path((Vector2)velocity, setVelocity, getPosition))
                {
                    if (!_bulletData.IsActive[i])
                    {
                        yield break;
                    }
                    yield return step;
                }
            }

            StartCoroutine(moveBulletWhileActive());
        }

        public void SpawnPattern(BulletPattern pattern)
        {
            StartCoroutine(pattern(this));
        }

        private IEnumerator ThrowCirclesAtPlayer(float2 startPos, float speed, float period)
        {
            for (;;)
            {
                float2 velocity =
                    math.normalize((float2)(Vector2)playerTransform.position - startPos) * speed;

                SpawnPattern(
                    Patterns.ThrowCircle(
                        radius: 50,
                        count: 20,
                        center: startPos,
                        velocity: velocity
                    )
                );

                yield return new WaitForSeconds(period);
            }
        }

        // ----------------------------
        // NEW: Enemy hit resolution
        // ----------------------------

        private void DespawnBullet(int i)
        {
            _bulletData.IsActive[i] = false;
            _bulletData.IsPlayerBullet[i] = false;
            _bulletData.Position[i] = new float2(MoveBulletJob.FAR_AWAY.x, MoveBulletJob.FAR_AWAY.y);
            _bulletData.Velocity[i] = float2.zero;

            if (_transformAccessArray.isCreated)
                _transformAccessArray[i].position = MoveBulletJob.FAR_AWAY;
        }

        // Bullet circle vs enemy AABB (box)
        private static bool CircleIntersectsAabb(float2 c, float r, float2 min, float2 max)
        {
            float cx = math.clamp(c.x, min.x, max.x);
            float cy = math.clamp(c.y, min.y, max.y);

            float dx = c.x - cx;
            float dy = c.y - cy;

            return (dx * dx + dy * dy) <= (r * r);
        }

        private void HandleEnemyHits()
        {
            var enemies = EnemyController.ActiveEnemies;
            if (enemies == null || enemies.Count == 0) return;

            for (int bi = 0; bi < maxBullets; bi++)
            {
                if (!_bulletData.IsActive[bi]) continue;

                // Only player bullets damage enemies
                if (!_bulletData.IsPlayerBullet[bi]) continue;

                float2 bp = _bulletData.Position[bi];
                float br = _bulletData.Radius[bi];

                for (int ei = 0; ei < enemies.Count; ei++)
                {
                    var enemy = enemies[ei];
                    if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

                    enemy.GetHurtboxAabb(out var min, out var max);

                    if (CircleIntersectsAabb(bp, br, min, max))
                    {
                        enemy.TakeDamage(playerBulletDamage);
                        DespawnBullet(bi);
                        break; // bullet consumed on first hit
                    }
                }
            }
        }
    }
}
