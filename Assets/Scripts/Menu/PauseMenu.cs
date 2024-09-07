using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    AudioManager audioManager;

    // Initialize AudioManager
    private void Awake()
    {
        audioManager = GameManager.instance.audioManager;
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    public void Resume()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        GameplayManager.instance.menuManager.HandlePause();
    }

    /// <summary>
    /// Restart the game
    /// </summary>
    public void RestartGame()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        GameManager.instance.inputHandler.ChangeActionMap("Player", 1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Load the main menu
    /// </summary>
    public void LoadMenu()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        GameManager.instance.inputHandler.ChangeActionMap("UI", 1f);
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}