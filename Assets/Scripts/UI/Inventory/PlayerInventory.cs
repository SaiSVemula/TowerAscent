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
        // Add a few test cards to the inventory for display
        AddCard(Resources.Load<Card>("Cards/Weapon Cards/Axe Chop"));
        AddCard(Resources.Load<Card>("Cards/Defence Cards/Dodge"));
        AddCard(Resources.Load<Card>("Cards/Magic Cards/Fireball"));
        AddCard(Resources.Load<Card>("Cards/Healing Cards/First Aid")); 
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
}
