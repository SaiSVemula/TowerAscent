//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerInventory : MonoBehaviour
//{
//    public static PlayerInventory Instance { get; private set; }

//    private List<Card> ownedCards = new List<Card>(); // Store the player's owned cards

//    // Event to notify when the inventory is updated
//    public event Action OnInventoryUpdated;

//    private void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        DontDestroyOnLoad(gameObject); // Persist between scenes
//    }

//    private void Start()
//    {
//        // Add a few test cards to the inventory for display - Testing
//        AddCard(Resources.Load<Card>("Cards/Weapon Cards/Axe Chop"));
//        AddCard(Resources.Load<Card>("Cards/Defence Cards/Dodge"));
//        AddCard(Resources.Load<Card>("Cards/Magic Cards/Fireball"));
//        AddCard(Resources.Load<Card>("Cards/Healing Cards/First Aid"));
//        AddCard(Resources.Load<Card>("Cards/Combination Cards/Dagger Dodge"));
//    }

//    public void AddCard(Card card)
//    {
//        if (card != null)
//        {
//            ownedCards.Add(card);
//            Debug.Log($"Card added to inventory: {card.Name}");

//            // Notify listeners (e.g., InventoryUITest) of the inventory update
//            OnInventoryUpdated?.Invoke();
//        }
//        else
//        {
//            Debug.LogWarning("Tried to add a null card to inventory.");
//        }
//    }

//    public List<Card> GetOwnedCards()
//    {
//        return new List<Card>(ownedCards); // Return a copy to prevent direct modification
//    }
//}

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private List<Card> ownedCards = new List<Card>(); // Store the player's owned cards

    // Event to notify when the inventory is updated
    public event Action OnInventoryUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist between scenes
    }

    private void Start()
    {
        LoadCardsFromGameManager(); // Load saved cards at the start of the game
    }

    public void AddCard(Card card)
    {
        if (card != null)
        {
            ownedCards.Add(card);
            Debug.Log($"Card added to inventory: {card.Name}");

            // Notify listeners (e.g., InventoryUITest) of the inventory update
            OnInventoryUpdated?.Invoke();
        }
        else
        {
            Debug.LogWarning("Tried to add a null card to inventory.");
        }
    }

    public List<Card> GetOwnedCards()
    {
        return new List<Card>(ownedCards); // Return a copy to prevent direct modification
    }

    public void LoadCardsFromGameManager()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance is null. Cannot load cards into PlayerInventory.");
            return;
        }

        string[] cardNames = GameManager.Instance.GetPlayerCards();
        if (cardNames == null || cardNames.Length == 0)
        {
            Debug.Log("No cards to load from GameManager.");
            return;
        }

        ownedCards.Clear();

        foreach (string cardName in cardNames)
        {
            Card card = Resources.Load<Card>(GetCardPath(cardName));
            if (card != null)
            {
                ownedCards.Add(card);
                Debug.Log($"Card loaded into inventory: {card.Name}");
            }
            else
            {
                Debug.LogWarning($"Failed to load card: {cardName}. Check Resources folder.");
            }
        }

        OnInventoryUpdated?.Invoke();
        Debug.Log($"Loaded {ownedCards.Count} cards into PlayerInventory.");
    }


    private string GetCardPath(string cardName)
    {
        // Weapon Cards
        if (cardName == "Axe Chop" || cardName == "Dagger Slash" || cardName == "Arrow Shot" ||
            cardName == "Spear Thrust" || cardName == "Hammer Smash" || cardName == "Sword Slash")
        {
            return $"Cards/Weapon Cards/{cardName}";
        }

        // Magic Cards
        if (cardName == "Fireball" || cardName == "Lightning Bolt" || cardName == "Ice Spike" ||
            cardName == "Earthquake" || cardName == "Chain Lightning" || cardName == "Meteor Shower")
        {
            return $"Cards/Magic Cards/{cardName}";
        }

        // Defence Cards
        if (cardName == "Dodge" || cardName == "Shield Block" || cardName == "Magic Barrier" ||
            cardName == "Reflect" || cardName == "Total Defense")
        {
            return $"Cards/Defence Cards/{cardName}";
        }

        // Healing Cards
        if (cardName == "First Aid" || cardName == "Healing Potion" || cardName == "Regen" ||
            cardName == "Group Heal")
        {
            return $"Cards/Healing Cards/{cardName}";
        }

        // Combination Cards
        if (cardName == "Dagger Dodge" || cardName == "Fire Sword" || cardName == "Healing Thrust" ||
            cardName == "Meteoric Defense" || cardName == "Ultimate Reflect")
        {
            return $"Cards/Combination Cards/{cardName}";
        }

        // Default fallback
        Debug.LogWarning($"Card name '{cardName}' does not match any known category. Using default path.");
        return $"Cards/{cardName}";
    }
}
