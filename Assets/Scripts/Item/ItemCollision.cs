using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollision : MonoBehaviour
{
    private Card ItemMagicCard;
    private Card ItemWeaponCard;
    private Card ItemDefenceCard;
    private Card ItemHealingCard;
    [SerializeField] private TextMeshProUGUI cardAddedMessage; // Reference to the UI TextMeshPro element
    [SerializeField] private float textDisplayDuration = 2f; // How long the message stays on screen

    private void Start()
    {
        // Load the card assets from the Resources folder
        ItemMagicCard = Resources.Load<Card>("Cards/Magic Cards/Fireball"); 
        ItemWeaponCard = Resources.Load<Card>("Cards/Weapon Cards/Axe Chop");
        ItemDefenceCard = Resources.Load<Card>("Cards/Defence Cards/Dodge");
        ItemHealingCard = Resources.Load<Card>("Cards/Healing Cards/First Aid");


        // Ensure the card message is disabled initially
        if (cardAddedMessage != null)
        {
            cardAddedMessage.text = "";
            cardAddedMessage.gameObject.SetActive(false); // Ensure it's disabled initially
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TextMeshPro textComponent = GetComponentInChildren<TextMeshPro>();
            if (textComponent != null)
            {
                string cardType = textComponent.text.Trim().ToUpper();

                // Assign a specific card based on the card type
                Card assignedCard = null;
                switch (cardType)
                {
                    case "MAGIC CARD":
                        assignedCard = ItemMagicCard;
                        break;
                    case "WEAPON CARD":
                        assignedCard = ItemWeaponCard;
                        break;
                    case "DEFENCE CARD":
                        assignedCard = ItemDefenceCard;
                        break;
                    case "HEALING CARD":
                        assignedCard = ItemHealingCard;
                        break;
                    default:
                        Debug.LogWarning($"Unrecognized card type: {cardType}");
                        break;
                }

                // Add the assigned card to the player's inventory if valid
                if (assignedCard != null)
                {
                    PlayerInventory.Instance.AddCard(assignedCard);
                    Debug.Log($"Player received a {cardType}: {assignedCard.Name}");

                    // Display the message on the screen
                    ShowCardAddedMessage(assignedCard.Name);
                }
                else
                {
                    Debug.LogWarning($"No card assigned for type: {cardType}");
                }
            }

            // Disable the item after collection
            gameObject.SetActive(false);
        }
    }

    private void ShowCardAddedMessage(string cardName)
    {
        if (cardAddedMessage != null)
        {
            // Update the message text and enable it
            cardAddedMessage.text = $"{cardName} added to inventory!";
            cardAddedMessage.gameObject.SetActive(true);

            // Schedule to hide the message after the duration
            Invoke(nameof(HideCardAddedMessage), textDisplayDuration);
        }
    }

    private void HideCardAddedMessage()
    {
        if (cardAddedMessage != null)
        {
            // Clear the text and hide the GameObject
            cardAddedMessage.text = "";
            cardAddedMessage.gameObject.SetActive(false);
        }
    }
}
