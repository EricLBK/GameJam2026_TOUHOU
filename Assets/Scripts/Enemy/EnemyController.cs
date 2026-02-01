using System;
using UnityEngine;


namespace Enemy
{
    public class EnemyController : MonoBehaviour
    { 
        [Header("Stats")]
        [SerializeField] private float maxHP = 10f;
        [SerializeField] private float speed = 0.5f; // 0.5 means 2 seconds to finish path
        
        private float _hitRadius; 
        private float _currentHP;
        private float _pathProgress; // 't' value (0.0 to 1.0)
        private EnemyPath _currentPath;
        private Rect _cullingRect;
        private Action<EnemyController> _onDeathCallback; // To return to pool

        // Initialize is called by the Manager when spawning
        public void Initialize(EnemyPath path, Rect cullRect, float hitRadius, Action<EnemyController> returnToPool)
        {
            _currentPath = path;
            _cullingRect = cullRect;
            _onDeathCallback = returnToPool;

            _currentHP = maxHP;
            _pathProgress = 0f;
            _hitRadius = hitRadius;
            
            // Reset Visuals
            gameObject.SetActive(true);
            transform.position = _currentPath.points[0];
        }

        void Update()
        {
            MoveAlongPath();
            CheckBounds();
        }

        private void MoveAlongPath()
        {
            if (_currentPath == null) return;

            // Advance 't' based on speed and time
            _pathProgress += speed * Time.deltaTime;

            // Calculate new position
            transform.position = _currentPath.Evaluate(_pathProgress);

            // Optional: Rotate to face direction
            // (Calculate vector between current pos and next frame pos)
        }

        private void CheckBounds()
        {
            // 1. Check if path is finished
            if (_pathProgress >= 1f)
            {
                Kill(false); // Despawn without explosion
                return;
            }

            // 2. Check if somehow drifted out of bounds (redundant for paths, but good safety)
            // if (!_cullingRect.Contains(transform.position))
            // {
            //     Kill(false);
            // }
        }

        // Call this when a bullet hits the enemy
        public void TakeDamage(float damage)
        {
            _currentHP -= damage;
            
            // Visual Feedback (Flash White)
            // StartCoroutine(FlashSprite()); 

            if (_currentHP <= 0)
            {
                Kill(true); // Die with explosion
            }
        }

        private void Kill(bool explode)
        {
            if (explode)
            {
                // Instantiate Explosion Particle
                // Drop Loot
            }

            // Return to pool
            gameObject.SetActive(false);
            _onDeathCallback?.Invoke(this);
        }
    }
}
