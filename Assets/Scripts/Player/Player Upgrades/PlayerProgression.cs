using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgression : MonoBehaviour
{
    [SerializeField] List<BasePowerup> effects = new List<BasePowerup>();
    [SerializeField] List<GameObject> cards = new List<GameObject>(3);
    Dictionary<GameObject, BasePowerup> cardEffects = new Dictionary<GameObject, BasePowerup>();

    private void Awake()
    {
        foreach (GameObject card in cards)
        {
            if (card == null) return;

            if (card.TryGetComponent<Button>(out Button cardButton))
            {
                cardButton.onClick.AddListener(() => OnCardSelected(card));
                if (cardButton.TryGetComponent<Animator>(out Animator cardAnimator))
                    cardButton.onClick.AddListener(() => cardAnimator.Play("UpgradeSelected"));
            }
        }
    }

    public void ShowEffects()
    {
        if (effects == null || effects.Count <= 0) return;

        GameplayManager.instance.menuManager.ToggleMenu(gameObject, true);

        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        List<BasePowerup> tempEffects = new List<BasePowerup>(effects);
        cardEffects.Clear();

        int loops = Mathf.Min(tempEffects.Count, cards.Count);

        for (int i = 0; i < loops; i++)
        {
            BasePowerup effect = tempEffects[Random.Range(0, tempEffects.Count)];

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

                Text cardText = cards[i].GetComponentInChildren<Text>();
                if (cardText != null)
                {
                    cardText.text = effect.PowerupName;
                    cardText.color = new Color(cardText.color.r, cardText.color.g, cardText.color.b, 1); 
                }
            }

            tempEffects.Remove(effect);
        }
    }
    void ApplyUpgrade(BasePowerup upgrade)
    {
        if (GameplayManager.instance.Player != null) upgrade.ApplyPowerup(GameplayManager.instance.Player);
    }

    public void OnCardSelected(GameObject selectedCard)
    {
        StartCoroutine(OnCardSelectedVisuals(selectedCard));
    }

    void OnCardSelectedLogic(GameObject selectedCard)
    {
        if (!cardEffects.ContainsKey(selectedCard)) return;

        BasePowerup selectedEffect = cardEffects[selectedCard];

        // Apply the selected effect
        ApplyUpgrade(selectedEffect);

        // Remove the selected effect from the main effects list
        effects.Remove(selectedEffect);

        // Clear the card-effect map for the next selection
        cardEffects.Clear();

        GameplayManager.instance.menuManager.ToggleMenu(gameObject, false);
    }

    IEnumerator OnCardSelectedVisuals(GameObject selectedCard)
    {
        if (!cardEffects.ContainsKey(selectedCard)) yield break;

        // Fade out the entire panel
        CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float alpha = canvasGroup.alpha;
            float timeElapsed = 0f;

            while (timeElapsed < 2)
            {
                timeElapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(alpha, 0f, timeElapsed / 2);
                yield return null;
            }
        }

        OnCardSelectedLogic(selectedCard);
        yield return null;
    }
}
