using System;
using System.Collections.Generic;
using Bullets.Patterns;
using UnityEngine;

namespace Bullets
{
    public class PatternEmitter : MonoBehaviour
    {
        private BulletManager _bulletManager;
        private BulletPattern _pattern;
        private float _timer;

        private void Start()
        {
            _bulletManager = gameObject.GetComponent<BulletManager>();
            _pattern = ScriptableObject.CreateInstance<SpiralPattern>();
        }

        private void Update()
        {
            if (_pattern == null)
                return;
            _timer += Time.deltaTime;
            var interval = 1f / _pattern.bulletsPerSecond;
            while (_timer >= interval)
            {
                _pattern.Execute(_bulletManager, transform, Time.time);
                _timer -= interval;
            }
        }
    }
}