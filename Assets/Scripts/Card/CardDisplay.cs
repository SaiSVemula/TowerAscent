using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card CardData { get; private set; } // Reference to the Card scriptable object

    [SerializeField] private Image cardImage; // UI Image to display the card's sprite
    //[SerializeField] private Text cardNameText; // UI Text to display the card's name

    // Initialize card UI with the provided card data
    public void Initialize(Card card)
    {
        CardData = card;

        // Check if CardSprite is null
        if (CardData.CardSprite == null)
        {
            Debug.LogError($"Card sprite is null for {CardData.Name}!");
            return; // Prevent further execution
        }

        // Update UI elements
        if (cardImage != null)
        {
            cardImage.sprite = CardData.CardSprite; // Use the card's sprite
        }
    }

    public Card GetCard()
    {
        return CardData;
    }
}
