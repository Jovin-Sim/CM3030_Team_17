using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        GameplayManager.instance.menuManager.HandlePause();
    }

    public void RestartGame()
    {
        GameManager.instance.inputHandler.ChangeActionMap("Player", 1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        GameManager.instance.inputHandler.ChangeActionMap("UI", 1f);
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}