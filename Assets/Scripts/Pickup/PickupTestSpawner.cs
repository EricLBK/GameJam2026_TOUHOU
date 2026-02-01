using UnityEngine;

public class PickupTestSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject soulPrefab;
    public GameObject lifePrefab;
    public GameObject bombPrefab;

    [Header("Spawn")]
    public float radius = 500f;
    public int burstCount = 10;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Spawn(soulPrefab,Random.insideUnitCircle * radius);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Spawn(lifePrefab,Random.insideUnitCircle * radius);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Spawn(bombPrefab,Random.insideUnitCircle * radius);

        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < burstCount; i++)
                Spawn(soulPrefab, Random.insideUnitCircle * radius);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // quick reset for testing
            var s = PlayerStats.Instance;
            if (s != null)
            {
                s.score = 0;
                s.souls = 0;
                s.lives = 3;
                s.bombs = 2;
            }
        }
    }

    void Spawn(GameObject prefab, Vector2 offset = default)
    {
        if (prefab == null) return;
        

        Vector2 pos = (Vector2)transform.position + offset;
        Instantiate(prefab, pos, Quaternion.identity);

        Debug.Log($"Spawned {prefab.name} at {pos}");
    }
}
