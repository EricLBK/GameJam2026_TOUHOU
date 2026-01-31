// attach this on an empty GameObject in your first scene to test

using UnityEngine;

public class SoulTestSpawner : MonoBehaviour
{
    [SerializeField] public int count = 1;
    [SerializeField] public int value = 50;
    [SerializeField] public float radius = 500f;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.T)) return;

        Vector2 center = transform.position;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = center + Random.insideUnitCircle * radius;
            ItemManager.Instance.SpawnSoul(pos);
            Debug.Log("Spawned soul at: " + pos);
        }
    }
}
