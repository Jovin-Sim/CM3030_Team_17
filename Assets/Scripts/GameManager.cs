using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public PlayerInputHandler inputHandler = null;
    public AudioManager audioManager = null;

    private void Awake()
    {
        if (GameObject.Find("GameManager") && GameObject.Find("GameManager") != gameObject)
        {
            Destroy(gameObject);
            return;
        }
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        inputHandler = gameObject.GetComponent<PlayerInputHandler>();
        audioManager = gameObject.GetComponent<AudioManager>();
    }
}
