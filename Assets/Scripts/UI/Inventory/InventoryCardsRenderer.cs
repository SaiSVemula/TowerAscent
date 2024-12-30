using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For scene management
using System.Collections.Generic;

public class InventoryCardRenderer : MonoBehaviour
{
    [SerializeField] private Transform cardGrid; // Reference to the CardGrid
    [SerializeField] private Sprite defaultCardSprite; // Placeholder sprite for cards
    [SerializeField] private List<Card> cards; // Serialized card list for Unity Inspector

    // Add this variable to check the current scene
    private bool isLoadoutScene;

    void Start()
    {
        // Check if the current scene is "Loadout"
        isLoadoutScene = SceneManager.GetActiveScene().name == "Loadout";

        GenerateTestCards(); // Generate cards in UI
    }

    // Generate cards in the UI based on the serialized card list
    void GenerateTestCards()
    {
        foreach (Card card in cards)
        {
            if (card == null) continue;

            // Create a new card GameObject
            GameObject newCard = new GameObject(card.Name, typeof(RectTransform), typeof(Image));

            // Set the parent to the CardGrid
            newCard.AddComponent<BoxCollider>();
            newCard.transform.SetParent(cardGrid, false);

            // Configure RectTransform
            RectTransform rect = newCard.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 200); // Card size
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            // Add and configure Image component
            Image img = newCard.GetComponent<Image>();
            img.sprite = card.CardSprite ?? defaultCardSprite; // Use the card's sprite or default
            img.color = Color.white;

            // Add a child for the card's text
            GameObject textObj = new GameObject("CardText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(newCard.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(150, 30); // Text size
            textRect.anchoredPosition = new Vector2(0, -85); // Position below the card

            // Configure the Text component
            Text text = textObj.GetComponent<Text>();
            text.text = card.Name; // Use the card's name
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;

            // Add DraggableItem only if in Loadout scene
            if (isLoadoutScene)
            {
                DraggableItem draggableItem = newCard.AddComponent<DraggableItem>();
                draggableItem.parentAfterDrag = cardGrid;

                // Add CardDisplay for additional functionality
                CardDisplay cardDisplay = newCard.AddComponent<CardDisplay>();
                cardDisplay.Initialize(card);
            }
        }
    }
}
