using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Bullets
{
    public class BulletManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxBullets = 2000;

        [SerializeField] private Texture2D bulletTexture;  
        [SerializeField] private Material baseMaterial; // The Unlit Instanced Material
        [SerializeField] private Rect bounds;
        private Mesh _quadMesh;
        private RenderParams _renderParams;
        
        // Data Containers
        private NativeArray<BulletData> _bulletData;
        private NativeArray<Matrix4x4> _renderMatrices; // Required for DrawMeshInstanced
        private NativeReference<int> _playerHitFlag;

        // Job Handles
        private JobHandle _moveHandle;
        private JobHandle _collisionHandle;

        // Player Reference (To pass to jobs)
        public Transform playerTransform;
        public float playerHitboxRadius = 0.1f;

        void Start()
        {
            // Allocation: Persistent because we reuse this memory every frame
            _bulletData = new NativeArray<BulletData>(maxBullets, Allocator.Persistent);
            _renderMatrices = new NativeArray<Matrix4x4>(maxBullets, Allocator.Persistent);
            _playerHitFlag = new NativeReference<int>(Allocator.Persistent);

            _quadMesh = GenerateQuadMesh();
            Material instanceMat = new Material(baseMaterial)
            {
                mainTexture = bulletTexture,
                enableInstancing = true // Ensure logic handles this
            };

            _renderParams = new RenderParams(instanceMat)
            {
                shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows = false
            };
        }

        void Update()
        {
            // 1. Complete previous frame's jobs (if you want strict consistency)
            // Or strictly schedule and complete within the frame.
            
            // --- SPAWN LOGIC (Simplified) ---
            // In a real game, you'd use a cursor to find the first inactive bullet index
            // and overwrite it with new data.
            SpawnTestBullets(); 

            // --- SCHEDULE MOVEMENT ---
            var moveJob = new MoveBulletJob()
            {
                DeltaTime = Time.deltaTime,
                BoundsMin = new float2(bounds.xMin, bounds.yMin),
                BoundsMax = new float2(bounds.xMax, bounds.yMax),
                Bullets = _bulletData
            };
            // Schedule parallelism based on list size (batches of 64 is standard)
            _moveHandle = moveJob.Schedule(maxBullets, 64);

            // --- SCHEDULE COLLISION (Depends on Movement) ---
            // Convert Player Pos to float2
            float2 playerPos = new float2(playerTransform.position.x, playerTransform.position.y);
            
            CollisionJob collisionJob = new CollisionJob
            {
                PlayerPosition = playerPos,
                PlayerRadiusSq = playerHitboxRadius * playerHitboxRadius,
                Bullets = _bulletData,
                HitDetected = _playerHitFlag
            };
            _collisionHandle = collisionJob.Schedule(maxBullets, 64, _moveHandle);

            UpdateMatricesJob matrixJob = new UpdateMatricesJob
            {
                Bullets = _bulletData,
                Matrices = _renderMatrices
            };
            JobHandle matrixHandle = matrixJob.Schedule(maxBullets, 64, _moveHandle);
            
            // --- EXECUTE & COMPLETE ---
            // We force completion here to render immediately. 
            // In highly optimized code, you might delay completion to LateUpdate.
            matrixHandle.Complete();
            _collisionHandle.Complete();

            // --- HANDLE RESULTS ---
            if (_playerHitFlag.Value == 1)
            {
                Debug.Log("PLAYER DEAD");
                _playerHitFlag.Value = 0; // Reset
            }
            // --- RENDER ---
                
            RenderBullets();
        }
        // helper to generate Quad mesh
        private Mesh GenerateQuadMesh()
        {
            var mesh = new Mesh
            {
                vertices = new[] {
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0),
                    new Vector3(0.5f, 0.5f, 0)
                },
                uv = new[] {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
                },
                triangles = new[] { 0, 2, 1, 2, 3, 1 }
            };
            return mesh;
        }

        private void RenderBullets()
        {
            _renderParams = new RenderParams
            {
                shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                receiveShadows = false,
                matProps = new MaterialPropertyBlock()
            };
            Graphics.RenderMeshInstanced(_renderParams, _quadMesh, 0, _renderMatrices);
        }

        private void OnDestroy()
        {
            // cleanup manual memory!
            if (_bulletData.IsCreated) _bulletData.Dispose();
            if (_renderMatrices.IsCreated) _renderMatrices.Dispose();
            if (_playerHitFlag.IsCreated) _playerHitFlag.Dispose();
        }

        // Temporary spawner for testing
        void SpawnTestBullets()
        {
            if (Input.GetKey(KeyCode.B))
            {
                for (int i = 0; i < maxBullets; i++)
                {
                    if (_bulletData[i].IsActive == 0)
                    {
                        BulletData b = _bulletData[i];
                        b.IsActive = 1;
                        b.Position = new float2(0, 5); // Spawn at top center
                        b.Velocity = new float2(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f));
                        b.Radius = 0.1f;
                        _bulletData[i] = b;
                        break; // Spawn one per frame
                    }
                }
            }
        }
    }
}