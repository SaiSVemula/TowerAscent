using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

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
        if (cardGrid == null)
        {
            Debug.LogError("CardGrid is not assigned in the Inspector!");
            return;
        }

        //Testing();

        RefreshInventoryUI(); // Display the initial inventory
    }

    private void Testing()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory.Instance is not initialized!");
            return;
        }

        // Add test cards to the inventory
        List<Card> testCards = new List<Card>
        {
            Resources.Load<Card>("Cards/Weapon Cards/Axe Chop"),
            Resources.Load<Card>("Cards/Magic Cards/Fireball"),
            Resources.Load<Card>("Cards/Defence Cards/Dodge"),
            Resources.Load<Card>("Cards/Healing Cards/First Aid"),
            Resources.Load<Card>("Cards/Combination Cards/Dagger Dodge")
        };

        foreach (Card card in testCards)
        {
            if (card == null)
            {
                Debug.LogError("A test card could not be loaded! Check the card path.");
            }
            else
            {
                PlayerInventory.Instance.AddCard(card);
            }
        }
    }

    public void RefreshInventoryUI()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory.Instance is null!");
            return;
        }

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

            if (SceneManager.GetActiveScene().name == "LoadoutPage")
            {
                // Add DraggableItem component
                newCard.AddComponent<DraggableItem>();
            }

            // Add CardDisplay component and initialize it
            CardDisplay cardDisplay = newCard.AddComponent<CardDisplay>();
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
