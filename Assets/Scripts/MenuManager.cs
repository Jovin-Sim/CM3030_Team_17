using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    bool isMenuOpen = false;

    public bool IsMenuOpen { get { return isMenuOpen; } }

    public void ToggleMenu(GameObject menu, bool openMenu)
    {
        if (menu == null) return;

        if (openMenu && !isMenuOpen)
        {
            menu.SetActive(true);
            isMenuOpen = true;
            Time.timeScale = 0.0f;
            if (GameplayManager.instance.Player != null)
            {
                GameplayManager.instance.Player.TogglePlayerControllability(false);
                GameplayManager.instance.Player.ToggleUIControllability(true);
            }
        }
        else
        {
            menu.SetActive(false);
            isMenuOpen = false;
            Time.timeScale = 1.0f;
            if (GameplayManager.instance.Player != null)
            {
                GameplayManager.instance.Player.TogglePlayerControllability(true);
                GameplayManager.instance.Player.ToggleUIControllability(false);
            }
        }
    }
}
