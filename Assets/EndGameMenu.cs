using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameMenu : MonoBehaviour
{
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject loseScreen;

    public void EndGame(bool win)
    {
        GameManager.instance.inputHandler.ChangeActionMap("UI", 0f);
        if (win && winScreen != null)
        {
            winScreen.SetActive(true);
            loseScreen.SetActive(false);
        }
        else if (loseScreen != null)
        {
            winScreen.SetActive(false);
            loseScreen.SetActive(true);
        }
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