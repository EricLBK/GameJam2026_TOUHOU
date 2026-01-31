using System;
using UnityEngine;
using UnityEngine.Jobs; // REQUIRED
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using System.Collections.Generic;

namespace Bullets
{
    public class BulletManager : MonoBehaviour
    {
        public Rect bounds;
        public int maxBullets = 2000;
        public GameObject bulletPrefab;
        
        public Transform playerTransform;
        public float playerHitBoxRadius;
        public float bulletRadius;

        // Data
        private NativeArray<BulletData> _bulletData;
        private TransformAccessArray _transformAccessArray; // Special array for Transforms
        private List<GameObject> _bulletPool; // Keep track of GOs to destroy later
        private NativeReference<int> _playerHitFlag;
        private int _nextBulletIndex;
        private Vector3 _spriteSize;

        private JobHandle _moveHandle;
        private JobHandle _collisionHandle;

        private void Start()
        {
            _bulletData = new NativeArray<BulletData>(maxBullets, Allocator.Persistent);
            _playerHitFlag = new NativeReference<int>(Allocator.Persistent);
            _bulletPool = new List<GameObject>(maxBullets);
            _nextBulletIndex = 0;
            
            var transforms = new Transform[maxBullets];
            for (var i = 0; i < maxBullets; i++)
            {
                var go = Instantiate(bulletPrefab, transform);
                var sr = bulletPrefab.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    _spriteSize = sr.sprite.bounds.size;
                    var newScale = new Vector3(
                        (bulletRadius * 2) / _spriteSize.x, 
                        (bulletRadius * 2) / _spriteSize.y, 
                        1f 
                    );
                    go.transform.localScale = newScale;
                }
                go.SetActive(true);
                _bulletPool.Add(go);
                transforms[i] = go.transform;
                
                // Initialize Data
                _bulletData[i] = new BulletData { IsActive = false };
            }

            // 2. Create the TransformAccessArray
            _transformAccessArray = new TransformAccessArray(transforms);
        }

        private void Update()
        {
            SpawnTestBullets();

            var moveJob = new MoveBulletJob
            {
                DeltaTime = Time.deltaTime,
                BoundsMin = new float2(bounds.xMin, bounds.yMin),
                BoundsMax = new float2(bounds.xMax, bounds.yMax),
                Bullets = _bulletData
            };

            _moveHandle = moveJob.Schedule(_transformAccessArray);
            var playerPos = new float2(playerTransform.position.x, playerTransform.position.y);

            var collisionJob = new CollisionJob
            {
                PlayerPosition = playerPos,
                PlayerRadiusSq = playerHitBoxRadius * playerHitBoxRadius,
                Bullets = _bulletData,
                HitDetected = _playerHitFlag
            };
            _collisionHandle = collisionJob.Schedule(maxBullets, 64, _moveHandle);
        }

        void LateUpdate()
        {
            _collisionHandle.Complete();
            if (_playerHitFlag.Value == 1)
            {
                
            }
        }

        public void SpawnBullet(Vector2 position, Vector2 velocity, float radius = 10.0f)
        {
            _collisionHandle.Complete();
            _moveHandle.Complete();
            
            for (var i = 0; i < maxBullets; i++)
            {
                var index = (_nextBulletIndex + 1) % maxBullets;
                if (_bulletData[index].IsActive) continue;
                
                var b = _bulletData[index];
                b.IsActive = true;
                b.Position = position;
                b.Velocity = velocity;
                b.Radius = radius;
                _bulletData[index] = b;

                var t = _bulletPool[index].transform;
                t.position = position;
                t.localScale = new Vector3(
                    (radius * 2) / _spriteSize.x, 
                    (radius * 2) / _spriteSize.y, 
                    1f 
                );
                
                _nextBulletIndex = (index + 1) % maxBullets;
                return;
            }
        }

        private void OnDestroy()
        {
            _collisionHandle.Complete();
            _moveHandle.Complete();
            if (_bulletData.IsCreated) _bulletData.Dispose();
            if (_playerHitFlag.IsCreated) _playerHitFlag.Dispose();
            if (_transformAccessArray.isCreated) _transformAccessArray.Dispose();
        }

        void SpawnTestBullets()
        {
             if (!Input.GetKey(KeyCode.B)) return;
             for (int i = 0; i < maxBullets; i++)
             {
                 if (!_bulletData[i].IsActive)
                 {
                     var b = _bulletData[i];
                     b.IsActive = true;
                     b.Velocity = new float2(UnityEngine.Random.Range(-100, 100f), UnityEngine.Random.Range(-100, 100f));
                     _bulletData[i] = b;
                     // Manually place the bullet for the start
                     _transformAccessArray[i].position = new Vector3(0, 300, 0); 
                     break; 
                 }
             }
        }
    }
}
