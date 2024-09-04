using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject instructionsPanel;
    AudioManager audioManager;

    // Initialize AudioManager
    private void Awake()
    {
        audioManager = GameManager.instance.audioManager;
    }

    public void StartGame()
    {
        // Load the main game scene (replace "GameScene" with your actual scene name)
        audioManager.PlaySFX(audioManager.buttonSelect);
        GameManager.instance.inputHandler.ChangeActionMap("Player", 1f);
        SceneManager.LoadScene("ReyScene");
    }

    public void ShowInstructions()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        instructionsPanel.SetActive(true);
    }

    public void CloseInstructions()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        instructionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        audioManager.PlaySFX(audioManager.buttonSelect);
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
