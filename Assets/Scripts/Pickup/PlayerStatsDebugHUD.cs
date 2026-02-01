using UnityEngine;

public class PlayerStatsDebugHUD : MonoBehaviour
{
    PlayerStats s;

    void Start()
    {
        s = PlayerStats.Instance;
    }

    void OnGUI()
    {
        if (s == null) return;

        GUI.Label(new Rect(10, 10, 400, 25), $"Score: {s.score}");
        GUI.Label(new Rect(10, 30, 400, 25), $"Souls: {s.souls}");
        GUI.Label(new Rect(10, 50, 400, 25), $"Lives: {s.lives}");
        GUI.Label(new Rect(10, 70, 400, 25), $"Bombs: {s.bombs}");
        GUI.Label(new Rect(10, 90, 600, 25), "Press 1=Soul, 2=Life, 3=Bomb, T=burst, R=reset");
    }
}
