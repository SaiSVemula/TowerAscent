using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card CardData { get; set; } // Reference to the Card scriptable object

    [SerializeField] private Image cardImage; // UI Image to display the card's sprite
    [SerializeField] private Text cardNameText; // UI Text to display the card's name

    // Initialize card UI with the provided card data
    public void Initialize(Card card)
    {
        CardData = card;

        if (cardImage != null && card.CardSprite != null)
        {
            cardImage.sprite = card.CardSprite; // Use the card's sprite
        }
        else
        {
            Debug.LogError($"Card sprite is null for {card.Name}!");
        }

        if (cardNameText != null)
        {
            cardNameText.text = card.Name; // Display the card's name
        }
    }


    public Card GetCard()
    {
        return CardData;
    }
}
