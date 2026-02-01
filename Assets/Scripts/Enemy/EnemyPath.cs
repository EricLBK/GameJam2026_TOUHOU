using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "NewEnemyPath", menuName = "Danmaku/Enemy Path")]
    public class EnemyPath : ScriptableObject
    {
        [Header("Bezier Control Points")]
        // P0 = Start, P1 = Control A, P2 = Control B, P3 = End
        public Vector2[] points = new Vector2[4]; 

        // Calculates the position at time 't' (0 to 1)
        public Vector2 Evaluate(float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
        
            // Cubic Bezier Formula
            return
                (oneMinusT * oneMinusT * oneMinusT * points[0]) +
                (3f * oneMinusT * oneMinusT * t * points[1]) +
                (3f * oneMinusT * t * t * points[2]) +
                (t * t * t * points[3]);
        }
    }
}
