using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;
    [SerializeField] Text scoreText;

    AudioManager audioManager;

    // Initialize AudioManager
    private void Awake()
    {
        audioManager = GameManager.instance.audioManager;
    }

    /// <summary>
    /// End the game
    /// </summary>
    /// <param name="win">The win status</param>
    public void EndGame(bool win)
    {
        // Change the score text
        if (scoreText != null) scoreText.text = "Score: " + GameplayManager.instance.zoneProgression.Score;

        // Display the appropriate screen
        GameManager.instance.inputHandler.ChangeActionMap("UI", 0f);
        if (win && winScreen != null)
        {
            GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.winGame);
            winScreen.SetActive(true);
            loseScreen.SetActive(false);
        }
        else if (loseScreen != null)
        {
            GameManager.instance.audioManager.PlaySFX(GameManager.instance.audioManager.playerDeath);
            winScreen.SetActive(false);
            loseScreen.SetActive(true);
        }
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