using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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
        SceneManager.LoadSceneAsync("");
    }
}
