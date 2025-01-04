using System.Collections.Generic;
using UnityEngine;
public class LoadoutButtons : MonoBehaviour
{
    // References to other components
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private Transform[] loadoutSlots; 
    [SerializeField] private LoadoutSlot[] cardSlots; 
    [SerializeField] private GameObject instructionsPanel;
    private List<string> validLoadout; 
    private List<Card> validLoadoutCards;

    // For the Clear button
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

    // for the Confirm Loadout button
    public void SaveLoadout()
    {
        validLoadout = new List<string>();
        validLoadoutCards = new List<Card>();

        int minimumRequiredCards = 4; // The minimum number of cards required to start the battle
        int validCardCount = 0;

        foreach (Transform slot in loadoutSlots)
        {
            if (slot.childCount > 0) // Check if the slot contains a card
            {
                CardDisplay cardDisplay = slot.GetChild(0).GetComponent<CardDisplay>(); // Get CardDisplay
                if (cardDisplay != null && cardDisplay.CardData != null)
                {
                    validLoadout.Add(cardDisplay.CardData.Name); // Add the card name to the valid loadout
                    validLoadoutCards.Add(cardDisplay.CardData); // Add the card object to the valid loadout
                    validCardCount++;
                }
                else
                {
                    Debug.LogWarning($"Invalid card in slot {slot.name}. Skipping this card.");
                    continue; // Skip invalid cards, but do not exit early
                }
            }
        }

        // Check if the minimum required cards are met
        if (validCardCount < minimumRequiredCards)
        {
            Debug.LogWarning($"Loadout is incomplete. A minimum of {minimumRequiredCards} cards is required.");
            return; // Exit early if the minimum card requirement is not met
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

    // To disable the instructions panel
    public void DisableInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            Debug.Log("Instructions panel has been disabled.");
        }
        else
        {
            Debug.LogWarning("Instructions panel is not assigned or already disabled.");
        }
    }

}
