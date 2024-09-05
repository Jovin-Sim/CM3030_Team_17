using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgression : MonoBehaviour
{
    // Store the powerup manager
    PowerupManager powerupManager;

    [Tooltip("The cards that displays the powerups")]
    [SerializeField] List<GameObject> cards = new List<GameObject>(3);

    // A dictionary containing the card objects and their powerups
    Dictionary<GameObject, BasePowerup> cardEffects = new Dictionary<GameObject, BasePowerup>();

    private void Awake()
    {
        // Get the powerupManager instance
        powerupManager = GameplayManager.instance.powerupManager;

        // Loop through each card in cards
        foreach (GameObject card in cards)
        {
            // Do nothing if the card is null
            if (card == null) return;

            // Check if the card has a button
            if (card.TryGetComponent<Button>(out Button cardButton))
            {
                // Add a listener to the card
                cardButton.onClick.AddListener(() => OnCardSelected(card));
            }
        }
    }

    /// <summary>
    /// Display the cards
    /// </summary>
    public void ShowEffects()
    {
        // Display the card menu
        GameplayManager.instance.menuManager.ToggleMenu(gameObject, true);

        // Set the alpha of the entire gameobject to 1
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        // Create a temporary list to store the available powerups
        List<BasePowerup> tempEffects = new List<BasePowerup>(powerupManager.AvailablePowerups);

        // Remove the explosives powerup from tempEffects
        foreach (BasePowerup powerup in tempEffects)
        {
            if (powerup.PowerupName == "Explosives Powerup")
            {
                tempEffects.Remove(powerup);
                break;
            }
        }

        // Clear the cardEffects dictionary
        cardEffects.Clear();

        for (int i = 0; i < cards.Count; i++)
        {
            // If there are no other powerups to get, disable the excess cards
            if (tempEffects.Count <= 0)
            {
                cards[i].gameObject.SetActive(false);
                continue;
            }
            else cards[i].gameObject.SetActive(true);

            // Get a random powerup
            BasePowerup effect = tempEffects[Random.Range(0, tempEffects.Count)];

            // Add it to cardEffects
            cardEffects[cards[i]] = effect;

            // Update the card's UI with the effect's name
            if (cards[i] != null)
            {
                // Enable the buttons
                Button cardButton = cards[i].GetComponent<Button>();
                if (cardButton != null) cardButton.interactable = true;

                // Ensure the cards are visible
                Image cardImage = cards[i].GetComponent<Image>();
                if (cardImage != null) cardImage.color = new Color(cardImage.color.r, cardImage.color.g, cardImage.color.b, 1);

                // Update the card text with the name of the powerup
                Text cardText = cards[i].GetComponentInChildren<Text>();
                if (cardText != null)
                {
                    cardText.text = effect.PowerupName;
                    cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1); 
                }
            }

            // Remove the effect from tempEffects to prevent duplicate powerups
            tempEffects.Remove(effect);
        }
    }

    /// <summary>
    /// Start the OnCardSelectedVisuals coroutine
    /// </summary>
    /// <param name="selectedCard">The selected card</param>
    public void OnCardSelected(GameObject selectedCard)
    {
        StartCoroutine(OnCardSelectedVisuals(selectedCard));
    }

    /// <summary>
    /// Execute the logic behind the card selection
    /// </summary>
    /// <param name="selectedCard">The selected card</param>
    void OnCardSelectedLogic(GameObject selectedCard)
    {
        // Do nothing if the card cannot be found
        if (!cardEffects.ContainsKey(selectedCard)) return;

        // Get the effect
        BasePowerup selectedEffect = cardEffects[selectedCard];

        // Apply the selected effect
        powerupManager.ApplyPowerup(selectedEffect);

        // Clear the card-effect map for the next selection
        cardEffects.Clear();

        // Disable the menu
        GameplayManager.instance.menuManager.ToggleMenu(gameObject, false);
    }

    /// <summary>
    /// Execute the visuals for the card selection
    /// </summary>
    /// <param name="selectedCard">The selected card</param>
    IEnumerator OnCardSelectedVisuals(GameObject selectedCard)
    {
        // Do nothing if the card cannot be found
        if (!cardEffects.ContainsKey(selectedCard)) yield break;

        // Find the card's animator and play the upgrade animation
        if (selectedCard.TryGetComponent<Animator>(out Animator anim))
            anim.SetTrigger("Upgrade");

        // Fade out the entire panel
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float alpha = canvasGroup.alpha;
            float timeElapsed = 0f;

            while (timeElapsed < 0.7f)
            {
                timeElapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(alpha, 0f, timeElapsed / 0.7f);
                yield return null;
            }
        }

        // Reset the card's animator's trigger
        if (anim != null)
        {
            anim.ResetTrigger("Upgrade");
        }

        // Execute the card selection logic
        OnCardSelectedLogic(selectedCard);
        yield return null;
    }
}