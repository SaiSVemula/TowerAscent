//old version
//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;

//// Temporary Card Class
//public class Card
//{
//    public string cardName; // Card name
//    public Sprite cardImage; // Card image
//    public int cardID; // Unique ID (optional)

//    public Card(string name, Sprite image, int id)
//    {
//        cardName = name;
//        cardImage = image;
//        cardID = id;
//    }
//}

//public class InventoryUITest : MonoBehaviour
//{
//    public Transform cardGrid; // Reference to the CardGrid
//    public Sprite defaultCardSprite; // Placeholder sprite for cards
//    private List<Card> temporaryCards; // List to hold temporary card data

//    void Start()
//    {
//        InitializeTemporaryCards(); // Create test card data
//        GenerateTestCards(); // Generate cards in UI
//    }

//    // Initialize the temporary card list
//    void InitializeTemporaryCards()
//    {
//        temporaryCards = new List<Card>();

//        for (int i = 0; i < 26; i++) // Simulate 10 cards
//        {
//            string name = $"Card {i + 1}";
//            Sprite image = defaultCardSprite; // Use the default sprite
//            int id = i + 1;

//            temporaryCards.Add(new Card(name, image, id));
//        }
//    }

//    // Generate cards in the UI based on the temporary card list
//    void GenerateTestCards()
//    {
//        foreach (Card card in temporaryCards)
//        {
//            // Create a new card GameObject
//            GameObject newCard = new GameObject(card.cardName, typeof(RectTransform), typeof(Image));

//            // Set the parent to the CardGrid
//            newCard.AddComponent<BoxCollider>();
//            newCard.transform.SetParent(cardGrid, false);

//            // Configure RectTransform
//            RectTransform rect = newCard.GetComponent<RectTransform>();
//            rect.sizeDelta = new Vector2(150, 200); // Card size
//            rect.anchorMin = new Vector2(0.5f, 0.5f);
//            rect.anchorMax = new Vector2(0.5f, 0.5f);
//            rect.pivot = new Vector2(0.5f, 0.5f);

//            // Add and configure Image component
//            Image img = newCard.GetComponent<Image>();
//            img.sprite = card.cardImage; // Use the card's image
//            img.color = Color.white;

//            // Add a child for the card's text
//            GameObject textObj = new GameObject("CardText", typeof(RectTransform), typeof(Text));
//            textObj.transform.SetParent(newCard.transform, false);
//            RectTransform textRect = textObj.GetComponent<RectTransform>();
//            textRect.sizeDelta = new Vector2(150, 30); // Text size
//            textRect.anchoredPosition = new Vector2(0, -85); // Position below the card

//            // Configure the Text component
//            Text text = textObj.GetComponent<Text>();
//            text.text = card.cardName; // Use the card's name
//            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
//            text.fontSize = 20;
//            text.alignment = TextAnchor.MiddleCenter;
//            text.color = Color.black;
//        }
//    }
//}

//new version
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUITest : MonoBehaviour
{
    public Transform cardGrid; // Reference to the CardGrid
    public Sprite defaultCardSprite; // Placeholder sprite for cards
    private List<Card> temporaryCards; // List to hold temporary card data

    void Start()
    {
        InitializeTemporaryCards(); // Create test card data
        GenerateTestCards(); // Generate cards in UI
    }

    // Initialize the temporary card list with your Card class and derived types
    void InitializeTemporaryCards()
    {
        temporaryCards = new List<Card>();

        // Simulate test cards using ScriptableObjects
        for (int i = 0; i < 5; i++) // Simulate a small sample of cards
        {
            Card card = Resources.Load<Card>($"Cards/Card{i + 1}"); // Load from Resources folder
            if (card != null)
            {
                temporaryCards.Add(card);
            }
        }
    }

    // Generate cards in the UI based on the temporary card list
    void GenerateTestCards()
    {
        foreach (Card card in temporaryCards)
        {
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
        }
    }
}
