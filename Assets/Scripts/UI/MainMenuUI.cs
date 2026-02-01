using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("NormalPlaythrough");
    }
    public void DemoGame()
    {
        SceneManager.LoadSceneAsync("DemoWalkthrough");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
