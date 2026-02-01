using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs; // REQUIRED

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

            SpawnPattern(
                Patterns.Spiral(
                    position: new float2(0, 200),
                    bulletSpeed: 200f,
                    duration: 1.0f,
                    path: Paths.Sine(amplitude: 0.5f, frequency: 2)
                )
            );
            // StartCoroutine(ThrowCirclesAtPlayer(startPos: new float2(0, 300), speed: 300f, period: 0.5f));
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
        }

        void LateUpdate()
        {
            _collisionHandle.Complete();
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

        public void SpawnBullet(
    float2 position,
    float2 velocity,
    BulletPath path = null,
    float radius = 50.0f
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


        void SpawnPattern(BulletPattern pattern)
        {
            StartCoroutine(pattern(this));
        }

        private IEnumerator ThrowCirclesAtPlayer(float2 startPos, float speed, float period)
        {
            for (; ; )
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
    }
}
