using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public Text cardNameText;
    public Text cardTypeText;
    public Text descriptionText;

    private void Start()
    {
        if (cardData != null)
        {
            cardNameText.text = cardData.cardName;
            cardTypeText.text = GetCardTypeText(cardData.cardType);
            descriptionText.text = cardData.description;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    // Method to get a readable string from the CardType enum
    private string GetCardTypeText(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.WeaponAttack:
                return "Attack - Weapon";
            case CardType.MagicAttack:
                return "Attack - Magic";
            case CardType.Defense:
                return "Defense";
            case CardType.Healing:
                return "Healing";
            default:
                return "Unknown Type";
        }
    }
}

