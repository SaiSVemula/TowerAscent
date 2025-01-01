using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InventoryCardRenderer : MonoBehaviour
{
    [SerializeField] private Transform cardGrid; // Reference to the CardGrid
    [SerializeField] private Sprite defaultCardSprite; // Placeholder sprite for cards

    private void OnEnable()
    {
        // Subscribe to the inventory updated event
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryUpdated += RefreshInventoryUI;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryUpdated -= RefreshInventoryUI;
        }
    }

    private void Start()
    {
        RefreshInventoryUI(); // Display the initial inventory
    }

    private void RefreshInventoryUI()
    {
        ClearInventoryUI(); // Clear existing UI

        List<Card> ownedCards = PlayerInventory.Instance.GetOwnedCards();


        foreach (Card card in ownedCards)
        {
            if (groupedCard.Card == null) continue;

            // Create a new card GameObject
            GameObject newCard = new GameObject(groupedCard.Card.Name, typeof(RectTransform), typeof(Image));

            // Set the parent to the CardGrid
            newCard.transform.SetParent(cardGrid, false);

            // Configure RectTransform
            RectTransform rect = newCard.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 200); // Card size
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            // Add and configure Image component
            Image img = newCard.GetComponent<Image>();
            img.sprite = groupedCard.Card.CardSprite ?? defaultCardSprite; // Use the card's sprite or default
            img.color = Color.white;

            // Add a child for the card's text
            GameObject textObj = new GameObject("CardText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(newCard.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(150, 30); // Text size
            textRect.anchoredPosition = new Vector2(0, -85); // Position below the card

            // Configure the Text component
            Text text = textObj.GetComponent<Text>();
            text.text = $"{groupedCard.Card.Name} x{groupedCard.Count}"; // Display card name with quantity
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;

            // if this is rendering in loadout then add the DraggableItem.cs
            if (SceneManager.GetActiveScene().name == "Loadout")
            {
                DraggableItem draggableItem = newCard.AddComponent<DraggableItem>();
                CardDisplay cardDisplay = newCard.AddComponent<CardDisplay>();
                cardDisplay.CardData = card; // Assign the card data
                draggableItem.parentAfterDrag = cardGrid; // Set the default parent
            }
        }
    }

    private void ClearInventoryUI()
    {
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }
    }
}
