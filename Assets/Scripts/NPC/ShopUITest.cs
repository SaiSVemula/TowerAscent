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
    private float messageDuration = 5f; // Duration for messages to display

    void Start()
    {
        // Validate required references
        if (cardGrid == null || messageText == null || goldCounter == null)
        {
            Debug.LogError("ShopUITest: One or more required references are missing in the Inspector!");
            return;
        }

        messageText.gameObject.SetActive(false); // Ensure MessageText starts disabled
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

            string cardName = string.IsNullOrEmpty(card.Name) ? "Unnamed Card" : card.Name;

            GameObject newCard = new GameObject(cardName, typeof(RectTransform), typeof(Image));
            newCard.transform.SetParent(cardGrid, false);

            RectTransform rect = newCard.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 200);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image img = newCard.GetComponent<Image>();
            img.sprite = card.CardSprite != null ? card.CardSprite : defaultCardSprite;
            img.color = Color.white;

            CardDisplay cardDisplay = newCard.AddComponent<CardDisplay>();
            cardDisplay.Initialize(card);

            Button button = newCard.AddComponent<Button>();
            button.onClick.AddListener(() => OnCardClicked(newCard));

            int price = Random.Range(5, 31);
            AddCardText(newCard, "CardPrice", $"Price: {price} Gold", new Vector2(0, -110), 18);

            cardPrices[newCard] = price;
        }
    }

    private void AddCardText(GameObject parent, string objName, string textContent, Vector2 anchoredPosition, int fontSize)
    {
        GameObject textObj = new GameObject(objName, typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(parent.transform, false);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 30);
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
        if (clickedCard == null || !cardPrices.ContainsKey(clickedCard))
        {
            Debug.LogError("Invalid card clicked.");
            return;
        }

        CardDisplay cardDisplay = clickedCard.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.CardData == null || PlayerInventory.Instance == null)
        {
            Debug.LogError("Missing card data or inventory reference.");
            return;
        }

        int cardPrice = cardPrices[clickedCard];

        if (clickedCard == lastClickedCard)
        {
            GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.GetPlayerCoinCount() - cardPrice);
            PlayerInventory.Instance.AddCard(cardDisplay.CardData);

            goldCounter.UpdateGoldText();
            ShowMessage($"You bought {cardDisplay.CardData.Name} for {cardPrice} Gold!");

            lastClickedCard = null;
        }
        else
        {
            ShowMessage($"Click again to buy {cardDisplay.CardData.Name} for {cardPrice} Gold.");
            lastClickedCard = clickedCard;
        }
    }

    private void ShowMessage(string text)
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = text;

            CancelInvoke(nameof(HideMessage)); // Reset timer if a new message is displayed
            Invoke(nameof(HideMessage), messageDuration);
        }
    }

    private void HideMessage()
    {
        if (messageText != null && messageText.gameObject.activeSelf)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }

    public void ResetUI()
    {
        // Hide MessageText when UI is closed
        if (messageText != null)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }

        // Reset last clicked card
        lastClickedCard = null;
        CancelInvoke(nameof(HideMessage)); // Cancel any pending HideMessage invokes
    }
}