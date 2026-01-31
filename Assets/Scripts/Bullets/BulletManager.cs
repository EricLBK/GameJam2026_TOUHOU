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
        public GameObject bulletPrefab; // Drag your Sprite Prefab here!
        public Transform playerTransform;

        // Data
        private NativeArray<BulletData> _bulletData;
        private TransformAccessArray _transformAccessArray; // Special array for Transforms
        private List<GameObject> _bulletPool; // Keep track of GOs to destroy later

        private JobHandle _moveHandle;

        void Start()
        {
            _bulletData = new NativeArray<BulletData>(maxBullets, Allocator.Persistent);
            _bulletPool = new List<GameObject>(maxBullets);
            
            // 1. Pre-instantiate all GameObjects (Object Pooling)
            var transforms = new Transform[maxBullets];
            for (int i = 0; i < maxBullets; i++)
            {
                GameObject go = Instantiate(bulletPrefab, transform); // Child of Manager
                go.SetActive(true); // Keep active, we hide them by moving them away
                _bulletPool.Add(go);
                transforms[i] = go.transform;
                
                // Initialize Data
                _bulletData[i] = new BulletData { IsActive = false };
            }

            // 2. Create the TransformAccessArray
            _transformAccessArray = new TransformAccessArray(transforms);
        }

        void Update()
        {
            SpawnTestBullets();

            // Schedule the Transform Job
            var moveJob = new MoveBulletJob
            {
                DeltaTime = Time.deltaTime,
                BoundsMin = new float2(bounds.xMin, bounds.yMin),
                BoundsMax = new float2(bounds.xMax, bounds.yMax),
                Bullets = _bulletData
            };

            // Note: Schedule is slightly different for TransformAccessArray
            _moveHandle = moveJob.Schedule(_transformAccessArray);
        }

        void LateUpdate()
        {
            _moveHandle.Complete();
        }

        private void OnDestroy()
        {
            _moveHandle.Complete();
            if (_bulletData.IsCreated) _bulletData.Dispose();
            if (_transformAccessArray.isCreated) _transformAccessArray.Dispose();
        }

        void SpawnTestBullets()
        {
            // Same logic as before, but ensure you set the Transform position too!
             // Since the Job runs every frame, setting _bulletData position isn't enough
             // The Job reads from Transform.position.

             if (!Input.GetKey(KeyCode.B)) return;
             for (int i = 0; i < maxBullets; i++)
             {
                 if (!_bulletData[i].IsActive)
                 {
                     BulletData b = _bulletData[i];
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
