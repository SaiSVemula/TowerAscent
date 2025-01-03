using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollision : MonoBehaviour
{
    private Dictionary<string, List<Card>> cardPools = new Dictionary<string, List<Card>>(); // Store card pools
    [SerializeField] private TextMeshProUGUI cardAddedMessage; // Reference to the UI TextMeshPro element
    [SerializeField] private float textDisplayDuration = 2f; // How long the message stays on screen

    private void Start()
    {
        // Load card pools from Resources folders
        cardPools["MAGIC CARD"] = LoadCardsFromFolder("Cards/Magic Cards");
        cardPools["WEAPON CARD"] = LoadCardsFromFolder("Cards/Weapon Cards");
        cardPools["DEFENCE CARD"] = LoadCardsFromFolder("Cards/Defence Cards");
        cardPools["HEALING CARD"] = LoadCardsFromFolder("Cards/Healing Cards");

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

                // Check if the card type exists in the dictionary
                if (cardPools.ContainsKey(cardType) && cardPools[cardType].Count > 0)
                {
                    // Get a random card from the respective pool
                    Card randomCard = GetRandomCardFromPool(cardPools[cardType]);

                    // Add the card to the player's inventory
                    if (randomCard != null)
                    {
                        PlayerInventory.Instance.AddCard(randomCard);
                        Debug.Log($"Player received a {cardType}: {randomCard.Name}");

                        // Display the message on the screen
                        ShowCardAddedMessage(randomCard.Name);
                    }
                    else
                    {
                        Debug.LogWarning($"No cards available in the pool for {cardType}.");
                    }
                }
                else
                {
                    Debug.LogWarning($"No card pool found for type: {cardType}");
                }
            }

            // Disable the item after collection
            gameObject.SetActive(false);
        }
    }

    private List<Card> LoadCardsFromFolder(string folderPath)
    {
        // Load all cards from the specified Resources folder
        Card[] cards = Resources.LoadAll<Card>(folderPath);
        Debug.Log($"Loaded {cards.Length} cards from {folderPath}");
        return new List<Card>(cards);
    }

    private Card GetRandomCardFromPool(List<Card> cardPool)
    {
        if (cardPool == null || cardPool.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, cardPool.Count);
        return cardPool[randomIndex];
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
