using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    public string upgradeType;
    public Text cardText;
    public float value;

    void Start()
    {

        // Update the text on the card if applicable
        if (cardText != null)
        {
            cardText.text = upgradeType;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Upgrade(upgradeType, value); // Call a method on the player to handle the upgrade
                Destroy(gameObject); // Destroy the UpgradeCard object after it's collected
            }
        }
    }
}
