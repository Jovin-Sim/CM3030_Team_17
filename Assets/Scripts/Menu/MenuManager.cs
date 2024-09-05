using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // A boolean to check if the menu is open
    bool isMenuOpen = false;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] EndGameMenu endGameMenuUI;

    public bool IsMenuOpen { get { return isMenuOpen; } }

    private void Start()
    {
        // Add the HandlePause listener
        GameManager.instance.inputHandler.OnPause += HandlePause;
    }

    /// <summary>
    /// Toggle a specified menu
    /// </summary>
    /// <param name="menu">The menu to toggle</param>
    /// <param name="openMenu">The toggle</param>
    public void ToggleMenu(GameObject menu, bool openMenu)
    {
        // Do nothing if the menu is null
        if (menu == null) return;

        // Get the input handler
        PlayerInputHandler inputHandler = GameManager.instance.inputHandler;
        // Log an error and return if the input handler is null
        if (inputHandler == null)
        {
            Debug.LogError("Input handler not found!");
            return;
        }

        // Open the menu
        if (openMenu && !isMenuOpen)
        {
            menu.SetActive(true);
            isMenuOpen = true;
            inputHandler.ChangeActionMap("UI", 0f);
        }
        // Close the menu
        else
        {
            menu.SetActive(false);
            isMenuOpen = false;
            inputHandler.ChangeActionMap("Player", 1f);
        }
    }

    /// <summary>
    /// Handle pausing
    /// </summary>
    public void HandlePause()
    {
        // Toggle the pause menu
        ToggleMenu(pauseMenuUI, !isMenuOpen);
    }

    /// <summary>
    /// End the game
    /// </summary>
    /// <param name="win">The win status</param>
    public void EndGame(bool win)
    {
        // Open the end game menu
        ToggleMenu(endGameMenuUI.transform.parent.gameObject, true);
        endGameMenuUI.EndGame(win);
    }

    void OnDestroy()
    {
        // Remove the HandlePause listener
        GameManager.instance.inputHandler.OnPause -= HandlePause;
    }
}