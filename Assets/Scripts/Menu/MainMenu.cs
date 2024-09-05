using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject instructionsPanel;
    AudioManager audioManager;

    // Initialize AudioManager
    private void Start()
    {
        audioManager = GameManager.instance.audioManager;
    }

    /// <summary>
    /// Start the game
    /// </summary>
    public void StartGame()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        GameManager.instance.inputHandler.ChangeActionMap("Player", 1f);
        SceneManager.LoadScene("ReyScene");
    }

    /// <summary>
    /// Show the instructions page
    /// </summary>
    public void ShowInstructions()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        instructionsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the instructions page
    /// </summary>
    public void CloseInstructions()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        instructionsPanel.SetActive(false);
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
