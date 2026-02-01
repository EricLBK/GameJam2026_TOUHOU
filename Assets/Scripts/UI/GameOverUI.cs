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
        SceneManager.LoadSceneAsync("TylerScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
