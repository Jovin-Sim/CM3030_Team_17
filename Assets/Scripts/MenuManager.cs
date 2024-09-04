using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    bool isMenuOpen = false;

    [SerializeField] GameObject pauseMenuUI;

    public bool IsMenuOpen { get { return isMenuOpen; } }

    private void Start()
    {
        GameManager.instance.inputHandler.OnPause += HandlePause;
    }

    public void ToggleMenu(GameObject menu, bool openMenu)
    {
        if (menu == null) return;

        PlayerInputHandler inputHandler = GameManager.instance.inputHandler;
        if (inputHandler == null)
        {
            Debug.LogError("Input handler not found!");
            return;
        }

        if (openMenu && !isMenuOpen)
        {
            menu.SetActive(true);
            isMenuOpen = true;
            inputHandler.ChangeActionMap("UI", 0f);
        }
        else
        {
            menu.SetActive(false);
            isMenuOpen = false;
            inputHandler.ChangeActionMap("Player", 1f);
        }
    }

    public void HandlePause()
    {
        ToggleMenu(pauseMenuUI, !isMenuOpen);
    }
}
