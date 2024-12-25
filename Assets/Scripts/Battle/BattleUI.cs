using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private Transform cardContainer; // The container where cards will be rendered
    [SerializeField] private GameObject cardTemplate; // A single card template in the UI

    private List<GameObject> renderedCards = new List<GameObject>();

    public void RenderCards(List<Card> cards)
    {
        // Clear existing cards
        foreach (GameObject cardObject in renderedCards)
        {
            Destroy(cardObject);
        }
        renderedCards.Clear();

        // Define spacing and alignment
        float cardWidth = 150f; // Width of each card
        float spacing = 20f;   // Gap between cards
        float startX = -(cards.Count * (cardWidth + spacing)) / 2 + (cardWidth / 2); // Center alignment

        for (int i = 0; i < cards.Count; i++)
        {
            Card currentCard = cards[i]; // Capture the current card

            // Create the card GameObject
            GameObject cardObject = new GameObject(currentCard.Name);
            cardObject.transform.SetParent(cardContainer, false); // Set parent to CardsPanel

            // Add components
            Image cardImage = cardObject.AddComponent<Image>();
            cardImage.sprite = currentCard.CardSprite;

            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(cardWidth, 200f); // Card dimensions

            // Set local position for alignment within the CardsPanel
            rectTransform.localPosition = new Vector3(startX + i * (cardWidth + spacing), 0, 0);

            // Add button interaction
            Button cardButton = cardObject.AddComponent<Button>();
            cardButton.onClick.AddListener(() => OnCardClicked(currentCard)); // Use captured card

            renderedCards.Add(cardObject);
        }
    }



    private void OnCardClicked(Card card)
    {
        Debug.Log($"Card clicked: {card.Name}");
        // Implement further card click behavior here
    }
}
