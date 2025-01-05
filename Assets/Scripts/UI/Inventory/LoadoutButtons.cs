using UnityEngine;
using System.Collections.Generic;

public class LoadoutButtons : MonoBehaviour
{
    [SerializeField] private Transform cardsGrid; // Reference to the cards grid
    [SerializeField] private Transform companionsGrid; // Reference to the companions grid
    [SerializeField] private Transform[] loadoutSlots; // All the slots in the loadout
    [SerializeField] private LoadoutSlot[] cardSlots; // Array of card slots
    [SerializeField] private GameObject instructionsPanel;

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
                loadoutSlot.SetCardTextColor(Color.black); // Reset slot text color

                if (loadoutSlot.slotType == "CompanionCard")
                {
                    // Return the companion card to the companions grid
                    ReturnCardToGrid(card.gameObject, companionsGrid);
                }
                else
                {
                    // Return other cards to the cards grid
                    ReturnCardToGrid(card.gameObject, cardsGrid);
                }
            }
        }
    }

    private void ReturnCardToGrid(GameObject cardObject, Transform targetGrid)
    {
        cardObject.transform.SetParent(targetGrid);
        cardObject.transform.localPosition = Vector3.zero; // Reset position in the grid
        RectTransform rect = cardObject.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(150, 200); // Reset size to default
        }
        Debug.Log($"Card {cardObject.name} returned to {targetGrid.name}.");
    }

    // For the Confirm Loadout button
    public void SaveLoadout()
    {
        List<Card> validLoadoutCards = new List<Card>();
        CompanionCard selectedCompanion = null;

        foreach (Transform slot in loadoutSlots)
        {
            if (slot.childCount > 0)
            {
                if (slot.name == "CompanionSlot")
                {
                    // Handle companion card
                    CompanionCardDisplay companionDisplay = slot.GetChild(0).GetComponent<CompanionCardDisplay>();
                    if (companionDisplay != null)
                    {
                        selectedCompanion = companionDisplay.CompanionCardData;
                        Debug.Log($"Companion selected: {selectedCompanion.CompanionName}");
                    }
                }
                else
                {
                    // Handle normal cards
                    CardDisplay cardDisplay = slot.GetChild(0).GetComponent<CardDisplay>();
                    if (cardDisplay != null && cardDisplay.CardData != null)
                    {
                        validLoadoutCards.Add(cardDisplay.CardData);
                    }
                }
            }
        }

        // Update the GameManager with the selected cards and companion
        GameManager.Instance.CurrentCardLoadout = validLoadoutCards;
        GameManager.Instance.AddCompanion(selectedCompanion);

        Debug.Log("Loadout saved successfully!");
    }

    // To disable the instructions panel
    public void DisableInstructionsPanel()
    {
        if (instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
            Debug.Log("Instructions panel has been disabled.");
        }
    }
}
