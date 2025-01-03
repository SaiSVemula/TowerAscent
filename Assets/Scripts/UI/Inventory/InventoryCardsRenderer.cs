using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // For scene management
using System.Linq; // Required for grouping functionality

public class InventoryCardsRenderer : MonoBehaviour
{
    [SerializeField] private Transform cardGrid; // Reference to the CardGrid
    [SerializeField] private Sprite defaultCardSprite; // Placeholder sprite for cards
    public bool isPickingWeaponCards = false; // Flag for weapon card filtering


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

    public void RefreshInventoryUI()
    {
        ClearInventoryUI(); // Clear existing UI

        List<Card> ownedCards = PlayerInventory.Instance.GetOwnedCards();

        // Filter for weapon cards if in weapon picking mode
        if (isPickingWeaponCards)
        {
            ownedCards = ownedCards.Where(card => card is WeaponCard).ToList();
        }

        // Group cards by name and count the quantities
        var groupedCards = ownedCards
            .GroupBy(card => card.Name)
            .Select(group => new { Card = group.First(), Count = group.Count() })
            .ToList();

        foreach (var groupedCard in groupedCards)
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

            if (SceneManager.GetActiveScene().name == "Loadout")
            {
                // Add DraggableItem component
                DraggableItem draggableItem = newCard.AddComponent<DraggableItem>();
            }
            CardDisplay cardDisplay = newCard.AddComponent<CardDisplay>();
            // Initialize the card display
            cardDisplay.Initialize(groupedCard.Card);

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
