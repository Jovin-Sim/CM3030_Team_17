using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static instance to implement the singleton pattern
    public static GameManager instance = null;

    // References to managers in the game
    public PlayerInputHandler inputHandler = null;
    public AudioManager audioManager = null;

    private void Awake()
    {
        // Destroy any other instance of game manager
        if (GameObject.Find("GameManager") && GameObject.Find("GameManager") != gameObject)
        {
            Destroy(gameObject);
            return;
        }
        // Assign this as the instance if there are no other instances
        if (instance == null)
            instance = this;
        // If the current instance is not the same as this, destroy the current object
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Prevent the GameManager from being destroyed when loading new scenes
        DontDestroyOnLoad(gameObject);

        inputHandler = gameObject.GetComponent<PlayerInputHandler>();
        audioManager = gameObject.GetComponent<AudioManager>();
    }

    private void OnApplicationQuit()
    {
        Destroy(inputHandler);
        Destroy(audioManager);
        Destroy(gameObject);
    }
}
