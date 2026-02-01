using System;
using System.Collections.Generic;
using Bullets;
using Unity.Mathematics;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {

        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Rect bounds;
        [SerializeField] private float radius;
        private EnemyPath _testPath;
        private Queue<EnemyController> _pool;
        private Vector3 _scale;
        private BulletManager _bulletManager;

        private void Start()
        {
            _pool = new Queue<EnemyController>();
            _testPath = ScriptableObject.CreateInstance<EnemyPath>();
            _testPath.points = new[]
            {
                new Vector2(400, 400),
                new Vector2(100, 100),
                new Vector2(100, 100),
                new Vector2(600, 100)
            };
            var sr = enemyPrefab.GetComponent<SpriteRenderer>();
            if (sr == null) return;
            var spriteSize = sr.sprite.bounds.size;
            _scale = new Vector3(
                (radius * 2) / spriteSize.x,
                (radius * 2) / spriteSize.y,
                1f);
            _bulletManager = gameObject.GetComponent<BulletManager>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            var controller = GetFromPool();
            controller.Initialize(path: _testPath, bulletManager:_bulletManager, cullRect: bounds, hitRadius: radius, returnToPool: ReturnToPool);
        }

        private EnemyController GetFromPool()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }
            var go = Instantiate(enemyPrefab, transform);
            go.transform.localScale = _scale;
            return go.GetComponent<EnemyController>();
        }

        private void ReturnToPool(EnemyController enemyController)
        {
           _pool.Enqueue(enemyController); 
        }
    }
}