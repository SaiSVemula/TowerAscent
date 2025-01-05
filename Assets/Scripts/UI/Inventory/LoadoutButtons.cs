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

        CompanionCard selectedCompanion = null;

        foreach (Transform slot in loadoutSlots)
        {
            if (slot.childCount > 0)
            {
                if (slot.name == "CompanionSlot")
                {
                    // Handle companion card
                    var companionDisplay = slot.GetChild(0).GetComponent<CompanionCard>();
                    if (companionDisplay != null)
                    {
                        selectedCompanion = companionDisplay;
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
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid card in slot {slot.name}.");
                        return;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Slot {slot.name} is empty. Loadout is incomplete.");
                return;
            }
        }

        Debug.Log("Loadout confirmed!");
        foreach (string cardName in validLoadout)
        {
            Debug.Log($"Card: {cardName}");
        }

        GameManager.Instance.CurrentCardLoadout = validLoadoutCards;
        GameManager.Instance.AddCompanion(selectedCompanion);

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
