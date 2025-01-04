using System.Collections.Generic;
using UnityEngine;
public class LoadoutButtons : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader; // Reference to LevelLoader
    [SerializeField] private Transform inventoryGrid; // Reference to the inventory grid
    [SerializeField] private Transform[] loadoutSlots; // Array of the right-hand slots
    [SerializeField] private LoadoutSlot[] cardSlots; // Array of the left-hand slots
    private List<string> validLoadout; // Stores the card names of the valid loadout
    private List<Card> validLoadoutCards; // Stores the Card objects of the valid loadout

    public void ClearSlots()
    {
        for (int i = 0; i < loadoutSlots.Length; i++)
        {
            Transform slot = loadoutSlots[i];
            LoadoutSlot loadoutSlot = cardSlots[i];
            if (slot.childCount > 0) // Check if the slot contains a card
            {
                Transform card = slot.GetChild(0); // Get the card
                loadoutSlot.SetCardTextColor(Color.black);
                card.SetParent(inventoryGrid); // Move the card back to the inventory
                card.localPosition = Vector3.zero; // Reset card's position
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 150); // Reset card's size
                Debug.Log($"Card {card.name} returned to inventory.");
            }
        }
    }

    public void SaveLoadout()
    {
        validLoadout = new List<string>();
        validLoadoutCards = new List<Card>();   

        foreach (Transform slot in loadoutSlots)
        {
            if (slot.childCount > 0) // Check if the slot contains a card
            {
                CardDisplay cardDisplay = slot.GetChild(0).GetComponent<CardDisplay>(); // Get CardDisplay
                if (cardDisplay != null && cardDisplay.CardData != null)
                {
                    validLoadout.Add(cardDisplay.CardData.Name); // Add the card name to the valid loadout
                    validLoadoutCards.Add(cardDisplay.CardData); // Add the card object to the valid loadout
                }
                else
                {
                    Debug.LogWarning($"Invalid card in slot {slot.name}.");
                    return; // Exit early if any slot contains an invalid card
                }
            }
            else
            {
                Debug.LogWarning($"Slot {slot.name} is empty. Loadout is incomplete.");
                return; // Exit early if loadout is incomplete
            }
        }

        Debug.Log("Loadout confirmed!");
        foreach (string cardName in validLoadout)
        {
            Debug.Log($"Card: {cardName}");
        }

        // Transition to the battle scene using LevelLoader
        if (levelLoader != null)
        {
            GameManager.Instance.CurrentCardLoadout = validLoadoutCards; // Save the valid loadout
            levelLoader.LoadScene("LoadoutPage", "BattleScene"); // Replace "BattleScene" with the actual scene name
        }
        else
        {
            Debug.LogError("LevelLoader reference is missing. Cannot transition to the next scene.");
        }
    }

    private bool ValidateCardType(int slotIndex, Card card)
    {
        // Check if the card matches the expected type for the slot
        switch (slotIndex)
        {
            case 0: // First slot: WeaponCard
                return card is WeaponCard;
            case 1: // Second slot: MagicCard
                return card is MagicCard;
            case 2: // Third slot: DefenceCard
                return card is DefenceCard;
            case 3: // Fourth slot: HealingCard
                return card is HealingCard;
            default:
                return false;
        }
    }
}
