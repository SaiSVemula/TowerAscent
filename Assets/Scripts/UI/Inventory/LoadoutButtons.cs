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
        foreach (var slot in loadoutSlots)
        {
            if (slot.childCount > 0)
            {
                Transform card = slot.GetChild(0); // Get the card
                var loadoutSlot = slot.GetComponent<LoadoutSlot>();
                if (loadoutSlot != null)
                {
                    loadoutSlot.SetCardTextColor(Color.black);
                }

                card.SetParent(inventoryGrid); // Move the card back to the inventory
                card.localPosition = Vector3.zero; // Reset card's position
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 150); // Reset card's size
                Debug.Log($"Card {card.name} returned to inventory.");
            }
        }

        // Clear companion in GameManager
        GameManager.Instance.AddCompanion(null);
        Debug.Log("Companion selection cleared.");
    }

    // For the Confirm Loadout button
    public void SaveLoadout()
    {
        validLoadout = new List<string>();
        validLoadoutCards = new List<Card>();

        CompanionCard selectedCompanion = null;

        int mandatoryCardCount = 0; // Track how many mandatory cards are added
        int requiredMandatoryCards = 4; // The number of mandatory cards needed

        foreach (Transform slot in loadoutSlots)
        {
            if (slot.childCount > 0)
            {
                if (slot.name == "CompanionSlot")
                {
                    // Handle companion card
                    var companionDisplay = slot.GetChild(0).GetComponent<CompanionCardDisplay>();
                    if (companionDisplay != null && companionDisplay.CompanionCardData != null)
                    {
                        selectedCompanion = companionDisplay.CompanionCardData;
                        Debug.Log($"Companion selected: {selectedCompanion.CompanionName}");
                    }
                }
                else
                {
                    // Handle regular cards
                    CardDisplay cardDisplay = slot.GetChild(0).GetComponent<CardDisplay>();
                    if (cardDisplay != null && cardDisplay.CardData != null)
                    {
                        validLoadout.Add(cardDisplay.CardData.Name);
                        validLoadoutCards.Add(cardDisplay.CardData);
                        mandatoryCardCount++;
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid card in slot {slot.name}.");
                        return;
                    }
                }
            }
        }

        // Check if mandatory cards are complete
        if (mandatoryCardCount < requiredMandatoryCards)
        {
            Debug.LogWarning($"Loadout is incomplete. You need at least {requiredMandatoryCards} mandatory cards.");
            return;
        }

        Debug.Log("Loadout confirmed!");
        foreach (string cardName in validLoadout)
        {
            Debug.Log($"Card: {cardName}");
        }

        // Save the loadout in GameManager
        GameManager.Instance.CurrentCardLoadout = validLoadoutCards;
        GameManager.Instance.AddCompanion(selectedCompanion);

        // Transition to the battle scene
        if (levelLoader != null)
        {
            levelLoader.LoadScene("LoadoutPage", "BattleScene");
        }
        else
        {
            Debug.LogError("LevelLoader reference is missing.");
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
