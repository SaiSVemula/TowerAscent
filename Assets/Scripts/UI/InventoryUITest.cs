using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUITest : MonoBehaviour
{
    [SerializeField] private Transform cardGrid; // Reference to the CardGrid
    [SerializeField] private Sprite defaultCardSprite; // Placeholder sprite for cards
    [SerializeField] private List<Card> cards; // Serialized card list for Unity Inspector

    void Start()
    {
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

            newCard.AddComponent<DraggableItem>(); // Add DraggableItem component
        }
    }
}


//new version

//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using TMPro;

//public class InventoryUITest : MonoBehaviour
//{
//    [SerializeField] private Transform cardGrid; // Parent container for the cards
//    [SerializeField] private GameObject cardPrefab; // Card prefab with a count display
//    [SerializeField] private List<Card> cards; // List of cards in the inventory

//    void Start()
//    {
//        GenerateInventoryUI(); // Generate the inventory UI
//    }

//    void GenerateInventoryUI()
//    {
//        // Dictionary to store counts for each card
//        Dictionary<string, int> cardCounts = new Dictionary<string, int>();

//        // Iterate through the inventory and count duplicates
//        foreach (Card card in cards)
//        {
//            if (card == null) continue;

//            string cardName = card.Name; // Replace with unique property if Name is not unique

//            if (cardCounts.ContainsKey(cardName))
//            {
//                cardCounts[cardName]++;
//            }
//            else
//            {
//                cardCounts[cardName] = 1;
//            }
//        }

//        // Create UI elements for each unique card
//        foreach (var entry in cardCounts)
//        {
//            // Find the card in the inventory to get its data
//            Card cardData = cards.Find(c => c.Name == entry.Key);

//            if (cardData == null) continue;

//            // Instantiate a card UI prefab
//            GameObject newCard = Instantiate(cardPrefab, cardGrid);

//            // Set the card sprite
//            Image cardImage = newCard.GetComponent<Image>();
//            if (cardImage != null)
//            {
//                cardImage.sprite = cardData.CardSprite;
//            }

//            // Set the count text
//            TextMeshProUGUI countText = newCard.transform.Find("DuplicateText")?.GetComponent<TextMeshProUGUI>();
//            if (countText != null)
//            {
//                countText.text = entry.Value > 1 ? $"x{entry.Value}" : ""; // Only show count if more than 1
//            }
//        }
//    }
//}
