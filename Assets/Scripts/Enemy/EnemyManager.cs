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
        
        // CHANGE 1: Use a Dictionary to store separate pools for each enemy type
        private Dictionary<string, Queue<EnemyController>> _pools;
        
        private Vector3 _scale;
        private BulletManager _bulletManager;
        private EnemyController _curController;

        private void Start()
        {
            // CHANGE 2: Initialize the dictionary
            _pools = new Dictionary<string, Queue<EnemyController>>();
            _bulletManager = gameObject.GetComponent<BulletManager>();
        }

        public void SpawnEnemy(EnemyPath path, int scoreDrops, float radius, [CanBeNull] GameObject gamePrefab, float speed = 1.0f)
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
            else
            {
                Debug.LogError("Cannot find prefab");
            }
            
            var controller = GetFromPool(gamePrefab);
            controller.Initialize(path: path, bulletManager:_bulletManager, scoreDrops: scoreDrops, hitRadius: radius, speed: speed, returnToPool: ReturnToPool);
            _curController = controller;
        }

        public void SpawnPattern(BulletPattern pattern, float duration)
        {
            if(_curController != null)
                _curController.SpawnPattern(pattern, duration);
        }

        private EnemyController GetFromPool([CanBeNull] GameObject prefab)
        {
            // fallback if prefab is null
            var prefabToUse = prefab != null ? prefab : enemyPrefab;
            string key = prefabToUse.name;

            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<EnemyController>();
            }

            if (_pools[key].Count > 0)
            {
                return _pools[key].Dequeue();
            }

            var go = Instantiate(prefabToUse, transform);
            go.name = key; 
            go.transform.localScale = _scale;
            return go.GetComponent<EnemyController>();
        }

        private void ReturnToPool(EnemyController enemyController)
        {
            // CHANGE 3: Put it back in the correct pool based on its name
            string key = enemyController.gameObject.name;
            
            if (!_pools.ContainsKey(key))
            {
                _pools[key] = new Queue<EnemyController>();
            }
            
            _pools[key].Enqueue(enemyController); 
            
            // Disable object so it doesn't run while in pool
            enemyController.gameObject.SetActive(false);
        }
    }
}
