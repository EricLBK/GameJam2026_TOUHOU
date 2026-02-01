using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void ReturnnMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
