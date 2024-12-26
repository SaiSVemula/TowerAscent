using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Transform cardContainer; // The container where cards will be rendered
    [SerializeField] private GameObject cardTemplate; // A single card template in the UI
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private PlayerBattle playerBattle;
    [SerializeField] private EnemyBattle enemyBattle;

    private List<GameObject> renderedCards = new List<GameObject>();

    [SerializeField] private TextMeshProUGUI defenseTimerText; // Text for defense timer
    [SerializeField] private TextMeshProUGUI healingTimerText; // Text for healing timer

    public void RenderCards(List<Card> cards)
    {
        // Clear existing cards
        foreach (GameObject cardObject in renderedCards)
        {
            Destroy(cardObject);
        }
        renderedCards.Clear();

        // Define card spacing
        float cardWidth = 150f;
        float cardHeight = 200f;
        float spacing = 20f;
        float startX = -(cards.Count * (cardWidth + spacing)) / 2 + (cardWidth / 2);

        for (int i = 0; i < cards.Count; i++)
        {
            Card currentCard = cards[i];

            // Instantiate from the template
            GameObject cardObject = Instantiate(cardTemplate, cardContainer);
            cardObject.SetActive(true); // Enable the instantiated object

            // Configure RectTransform
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(cardWidth, cardHeight);
                rectTransform.localScale = Vector3.one * 0.7f; // Scale adjustment
                rectTransform.localPosition = new Vector3(startX + i * (cardWidth + spacing), 0, 0);
            }

            // Update card visuals
            Image cardImage = cardObject.GetComponent<Image>();
            if (cardImage != null && currentCard.CardSprite != null)
            {
                cardImage.sprite = currentCard.CardSprite;
            }

            Text cardNameText = cardObject.GetComponentInChildren<Text>();
            if (cardNameText != null)
            {
                cardNameText.text = currentCard.Name;
            }

            // Add button interaction
            Button cardButton = cardObject.GetComponent<Button>();
            if (cardButton != null)
            {
                int index = i;
                cardButton.onClick.AddListener(() => battleManager.OnPlayerUseCard(index));
            }

            renderedCards.Add(cardObject);
        }
    }

    // Update defense and healing timer displays
    public void UpdateEffectTimers()
    {
        if (defenseTimerText == null || healingTimerText == null)
        {
            Debug.LogError("Timer Text references are not assigned in the BattleUI script.");
            return;
        }

        // Defense Timer
        if (playerBattle.TemporaryDefenses.Count == 0)
        {
            defenseTimerText.text = "Defense: 0"; // Set text to zero immediately
            StartCoroutine(RemoveTextAfterDelay(defenseTimerText, 5f)); // Delay removal
        }
        else
        {
            string defenseText = "Defense:\n";
            foreach (var defense in playerBattle.TemporaryDefenses)
            {
                defenseText += $"Value: {defense.value}, Turns Left: {defense.timer}\n";
            }
            defenseTimerText.text = defenseText;
            defenseTimerText.gameObject.SetActive(true); // Ensure visibility
        }

        // Healing Timer
        if (playerBattle.TemporaryHeals.Count == 0)
        {
            healingTimerText.text = "Healing: 0"; // Set text to zero immediately
            StartCoroutine(RemoveTextAfterDelay(healingTimerText, 5f)); // Delay removal
        }
        else
        {
            string healingText = "Healing:\n";
            foreach (var heal in playerBattle.TemporaryHeals)
            {
                healingText += $"Value: {heal.value}, Turns Left: {heal.timer}\n";
            }
            healingTimerText.text = healingText;
            healingTimerText.gameObject.SetActive(true); // Ensure visibility
        }
    }

    // Remove text after a delay
    private IEnumerator RemoveTextAfterDelay(TextMeshProUGUI textElement, float delay)
    {
        yield return new WaitForSeconds(delay);
        textElement.gameObject.SetActive(false); // Hide the text after delay
        textElement.text = ""; // Clear the text content
    }
}