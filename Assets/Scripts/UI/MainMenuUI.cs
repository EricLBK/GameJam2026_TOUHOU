using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("TylerScene");
    }
    public void DemoGame()
    {
        SceneManager.LoadSceneAsync("MainScene_hacks");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
