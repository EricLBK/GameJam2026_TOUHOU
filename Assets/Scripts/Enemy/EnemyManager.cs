using System;
using System.Collections.Generic;
using Bullets;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {

        [SerializeField] private GameObject enemyPrefab;
        private Queue<EnemyController> _pool;
        private Vector3 _scale;
        private BulletManager _bulletManager;
        private EnemyController _curController;

        private void Start()
        {
            _pool = new Queue<EnemyController>();
            _bulletManager = gameObject.GetComponent<BulletManager>();
        }

        public void SpawnEnemy(EnemyPath path, int scoreDrops, float radius,[CanBeNull] GameObject gamePrefab, float speed = 1.0f)
        {
            if (gamePrefab != null)
            {
                var sr = gamePrefab.GetComponent<SpriteRenderer>();
                if (sr == null) return;
                var spriteSize = sr.sprite.bounds.size;
                _scale = new Vector3(
                    (radius * 2) / spriteSize.x,
                    (radius * 2) / spriteSize.y,
                    1f);
            }
            
            var controller = GetFromPool(gamePrefab);
            controller.Initialize(path: path, bulletManager:_bulletManager, scoreDrops: scoreDrops, hitRadius: radius, speed: speed, returnToPool: ReturnToPool);
            _curController = controller;
        }

        // Always make sure spawn enemy is called before this!
        public void SpawnPattern(BulletPattern pattern, float duration)
        {
            if(_curController != null)
                _curController.SpawnPattern(pattern, duration);
        }

        private EnemyController GetFromPool([CanBeNull] GameObject prefab)
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }

            var go = (prefab == null) ?  Instantiate(enemyPrefab, transform) : Instantiate(prefab, transform);
            go.transform.localScale = _scale;
            return go.GetComponent<EnemyController>();
        }

        private void ReturnToPool(EnemyController enemyController)
        {
           _pool.Enqueue(enemyController); 
        }
    }
}
