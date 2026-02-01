using UnityEngine;

public class PlayerRegistry : MonoBehaviour
{
    public static Transform PlayerTransform { get; private set; }
    public static PlayerStats PlayerStats { get; private set; }

    void Awake()
    {
        PlayerStats = GetComponent<PlayerStats>();
        PlayerTransform = transform;

        Debug.Log($"PlayerRegistry registered: {name} at {transform.position}");
    }
}
