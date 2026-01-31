using UnityEngine;

public class SoulTestSpawner : MonoBehaviour
{
    [SerializeField] public int count = 10;
    [SerializeField] public int value = 50;

    // For pixel-art worlds, keep this small at first (like 1–5 world units)
    [SerializeField] public float radius = 500f;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.T)) return;

        Vector2 center = transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = center + Random.insideUnitCircle * radius;
            SoulManager.Instance.Spawn(pos, value);
            Debug.Log("Spawned soul at: " + pos);
        }
    }
}
