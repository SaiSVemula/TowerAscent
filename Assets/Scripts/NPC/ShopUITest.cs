using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUITest : MonoBehaviour
{
    [SerializeField] private Transform cardGrid; // Reference to the CardGrid
    [SerializeField] private Sprite defaultCardSprite; // Placeholder sprite for cards
    [SerializeField] private List<Card> cards; // Serialized card list for Unity Inspector
    [SerializeField] private TextMeshProUGUI messageText; // Reference to the MessageText UI
    [SerializeField] private GoldCounter goldCounter; // Reference to the GoldCounter script

    private Dictionary<GameObject, int> cardPrices = new Dictionary<GameObject, int>(); // Store card prices
    private GameObject lastClickedCard; // Track the last clicked card

    void Start()
    {
        // Validate required references
        if (cardGrid == null || messageText == null || goldCounter == null)
        {
            Debug.LogError("ShopUITest: One or more required references are missing in the Inspector!");
            return;
        }

        GenerateShopCards(); // Generate cards in the shop UI
    }

    // Generate cards in the shop UI with prices
    void GenerateShopCards()
    {
        foreach (Card card in cards)
        {
            if (card == null)
            {
                Debug.LogWarning("Skipping a null card in the list.");
                continue;
            }

            // Ensure required properties have valid values
            string cardName = string.IsNullOrEmpty(card.Name) ? "Unnamed Card" : card.Name;

            // Create a new card GameObject
            GameObject newCard = new GameObject(cardName, typeof(RectTransform), typeof(Image));

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
            img.sprite = card.CardSprite != null ? card.CardSprite : defaultCardSprite;
            img.color = Color.white;

            // Add CardDisplay and initialize it
            CardDisplay cardDisplay = newCard.AddComponent<CardDisplay>();
            cardDisplay.Initialize(card);

            // Add a Button component for click handling
            Button button = newCard.AddComponent<Button>();
            button.onClick.AddListener(() => OnCardClicked(newCard));

            Debug.Log($"Button listener added for card: {card.Name}");

            // Add a child for the card's name
            AddCardText(newCard, "CardName", card.Name, new Vector2(0, -75), 20);

            // Add a child for the card's price
            int price = Random.Range(5, 31); // Random price between 5 and 30
            AddCardText(newCard, "CardPrice", $"Price: {price} Gold", new Vector2(0, -110), 18);

            // Store the card price
            cardPrices[newCard] = price;
        }
    }

    // Helper function to add text to a card
    private void AddCardText(GameObject parent, string objName, string textContent, Vector2 anchoredPosition, int fontSize)
    {
        GameObject textObj = new GameObject(objName, typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(parent.transform, false);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 30); // Text size
        rect.anchoredPosition = anchoredPosition;

        Text text = textObj.GetComponent<Text>();
        text.text = textContent;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
    }

    private void OnCardClicked(GameObject clickedCard)
    {
        if (clickedCard == null)
        {
            Debug.LogError("OnCardClicked: clickedCard is null.");
            return;
        }

        if (!cardPrices.ContainsKey(clickedCard))
        {
            Debug.LogError($"OnCardClicked: Card not found in cardPrices dictionary. Card name: {clickedCard.name}");
            return;
        }

        CardDisplay cardDisplay = clickedCard.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.CardData == null)
        {
            Debug.LogError("OnCardClicked: CardDisplay or CardData is null on clickedCard.");
            return;
        }

        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("OnCardClicked: PlayerInventory.Instance is null.");
            return;
        }

        int cardPrice = cardPrices[clickedCard];

        if (clickedCard == lastClickedCard)
        {
            // Deduct gold (always allow purchase regardless of available gold for testing)
            GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.GetPlayerCoinCount() - cardPrice);

            // Add the card to the player's inventory
            PlayerInventory.Instance.AddCard(cardDisplay.CardData);

            // Update the gold counter and UI
            goldCounter.UpdateGoldText();
            messageText.text = $"You bought {cardDisplay.CardData.Name} for {cardPrice} Gold!";

            // Reset the last clicked card to prevent repeat buys without another click
            lastClickedCard = null;
        }
        else
        {
            // Set the message to confirm purchase
            messageText.text = $"Click again to buy {cardDisplay.CardData.Name} for {cardPrice} Gold.";
            lastClickedCard = clickedCard;
        }
    }
}
