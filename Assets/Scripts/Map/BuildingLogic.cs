using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLogic : MonoBehaviour
{
    // The renderer of the game object
    Renderer roof;

    private void Awake()
    {
        // Get the game object's renderer
        roof = GetComponent<Renderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Do nothing if no collider was found
        if (collision == null) return;
        // Make the roof invisible if the player is within it
        if (collision.gameObject == GameplayManager.instance.Player.gameObject) roof.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Do nothing if no collider was found
        if (collision == null) return;
        // Make the roof visible if the player is outside of it
        if (collision.gameObject == GameplayManager.instance.Player.gameObject) roof.enabled = true;
    }
}
