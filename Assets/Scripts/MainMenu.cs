using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject instructionsPanel;
    AudioManager audioManager;

    // Initialize AudioManager
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void StartGame()
    {
        // Load the main game scene (replace "GameScene" with your actual scene name)
        audioManager.PlaySFX(audioManager.buttonSelect);
        SceneManager.LoadScene("MainScene");
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
