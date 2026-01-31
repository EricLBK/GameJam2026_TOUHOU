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

        [SerializeField] private Texture2D texture2D;  
        [SerializeField] private Mesh bulletMesh;
        [SerializeField] private Material baseMaterial; // The Unlit Instanced Material
        [SerializeField] private Rect bounds;

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

            // Pre-fill matrices to avoid 0,0,0 glitches (optional)
            // Initialize pool with inactive bullets
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

            // --- EXECUTE & COMPLETE ---
            // We force completion here to render immediately. 
            // In highly optimized code, you might delay completion to LateUpdate.
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

        private void RenderBullets()
        {
            // We need to copy active positions to matrices
            // NOTE: In a production environment, this copy should also be a Job!
            // Copying 2000 structs on the main thread is a bottleneck.
            
            // Array for Graphics.DrawMeshInstanced (limited to 1023 per batch usually, 
            // so we loop or use DrawMeshInstancedIndirect). 
            // For simplicity, here is the matrix construction logic:
            
            // This is the "Expensive" part on the Main Thread if not jobified:
            int activeCount = 0;
            Matrix4x4[] batch = new Matrix4x4[maxBullets]; // Heap allocation warning! Reuse this array in real code.
            
            for (int i = 0; i < maxBullets; i++)
            {
                if (_bulletData[i].IsActive == 1)
                {
                    var pos = _bulletData[i].Position;
                    // Create matrix (Translation, Rotation, Scale)
                    batch[activeCount] = Matrix4x4.TRS(
                        new Vector3(pos.x, pos.y, 0), 
                        Quaternion.identity, 
                        Vector3.one * 0.2f // Bullet Scale
                    );
                    activeCount++;
                }
            }

            // Draw
            if (activeCount > 0)
            {
                // Note: DrawMeshInstanced draws max 1023 meshes per call. You need to slice the array.
                // Or use RenderParams with Graphics.RenderMesh (Unity 2021+)
                for (int i = 0; i < activeCount; i += 1023)
                {
                    int count = Mathf.Min(1023, activeCount - i);
                    // Create a slice (pseudo-code, DrawMeshInstanced takes an array and offset)
                    Graphics.DrawMeshInstanced(bulletMesh, 0, baseMaterial, batch, count, null, UnityEngine.Rendering.ShadowCastingMode.Off, false, 0, null, UnityEngine.Rendering.LightProbeUsage.Off, null);
                }
            }
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
            if (Input.GetKey(KeyCode.Z))
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