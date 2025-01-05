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

    private List<Card> lastRenderedCards = new List<Card>(); // Track last rendered cards to prevent duplication

    private void OnEnable()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnInventoryUpdated += RefreshInventoryUI;
        }
    }

    private void OnDisable()
    {
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

        RefreshInventoryUI(); // Display the initial inventory
    }

    public void RefreshInventoryUI()
    {
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("PlayerInventory.Instance is null!");
            return;
        }

        List<Card> ownedCards = PlayerInventory.Instance.GetOwnedCards();

        // Filter for weapon cards if in weapon picking mode
        if (isPickingWeaponCards)
        {
            ownedCards = ownedCards.Where(card => card is WeaponCard).ToList();
        }

        // If the current owned cards match the last rendered cards, skip re-rendering
        if (ownedCards.SequenceEqual(lastRenderedCards))
        {
            Debug.Log("Cards are already up-to-date. Skipping re-rendering.");
            return;
        }

        // Save the current owned cards as the last rendered state
        lastRenderedCards = new List<Card>(ownedCards);

        ClearInventoryUI(); // Clear existing UI

        // Group cards by name and count the quantities
        var groupedCards = ownedCards
            .GroupBy(card => card.Name)
            .Select(group => new { Card = group.First(), Count = group.Count() })
            .ToList();

        foreach (var groupedCard in groupedCards)
        {
            if (groupedCard.Card == null) continue;

            // Check if CardSprite is null and log a warning
            if (groupedCard.Card.CardSprite == null)
            {
                Debug.LogWarning($"Card sprite is missing for {groupedCard.Card.Name}.");
                continue; // Skip rendering this card
            }

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