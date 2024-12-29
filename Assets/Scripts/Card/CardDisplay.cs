//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//using UnityEngine;

//public class CardDisplay : MonoBehaviour
//{
//    private int cardIndex;
//    private BattleManager battleManager;

//    // Initialize card UI with the provided card data.
//    public void Initialize(Card card, int index, BattleManager manager)
//    {
//        cardIndex = index;
//        battleManager = manager;
//    }

//    // Called when the card is clicked.
//    public void OnCardClicked()
//    {
//        if (battleManager != null)
//        {
//            battleManager.OnPlayerUseCard(cardIndex);
//        }
//    }
//}


using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card CardData { get; private set; } // Reference to the Card scriptable object

    [SerializeField] private Image cardImage; // UI Image to display the card's sprite
    [SerializeField] private Text cardNameText; // UI Text to display the card's name

    // Initialize card UI with the provided card data
    public void Initialize(Card card)
    {
        CardData = card;

        // Update UI elements
        if (cardImage != null)
        {
            cardImage.sprite = card.CardSprite; // Use the card's sprite
        }

        if (cardNameText != null)
        {
            cardNameText.text = card.Name; // Display the card's name
        }
    }
}
