using UnityEngine;

public class EnemyTestSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    [SerializeField] public int count = 1;
    [SerializeField] public int value = 50;
    [SerializeField] public float radius = 500f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector2 center = transform.position;

            for (int i = 0; i < count; i++)
            {
                Vector2 pos = center + Random.insideUnitCircle * radius;
                Instantiate(enemyPrefab, pos, Quaternion.identity);
                Debug.Log("Spawned soul at: " + pos);
            }
        }
            
    }
}
