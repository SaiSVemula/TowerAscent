using System.Collections.Generic;
using UnityEngine;

public class LoadoutButtons : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private Transform inventoryGrid;
    [SerializeField] private Transform[] loadoutSlots;
    [SerializeField] private LoadoutSlot[] cardSlots;
    [SerializeField] private GameObject instructionsPanel;

    private List<string> validLoadout;
    private List<Card> validLoadoutCards;

    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader reference is missing.");
        }
    }

    public void ClearSlots()
    {
        for (int i = 0; i < loadoutSlots.Length; i++)
        {
            Transform slot = loadoutSlots[i];
            LoadoutSlot loadoutSlot = cardSlots[i];
            if (slot.childCount > 0)
            {
                Transform card = slot.GetChild(0);
                loadoutSlot.SetCardTextColor(Color.black);
                card.SetParent(inventoryGrid);
                card.localPosition = Vector3.zero;
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 150);
                Debug.Log($"Card {card.name} returned to inventory.");
            }
        }
    }

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
                    var companionDisplay = slot.GetChild(0).GetComponent<CompanionCardDisplay>();
                    if (companionDisplay != null)
                    {
                        selectedCompanion = companionDisplay.CompanionCardData;
                        Debug.Log($"Companion selected: {selectedCompanion?.CompanionName ?? "None"}");
                    }
                }
                else
                {
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
        }

        Debug.Log("Loadout confirmed!");
        foreach (string cardName in validLoadout)
        {
            Debug.Log($"Card: {cardName}");
        }

        GameManager.Instance.CurrentCardLoadout = validLoadoutCards;

        if (selectedCompanion != null)
        {
            // Add the companion and set its type
            GameManager.Instance.AddCompanion(selectedCompanion);
            GameManager.Instance.SetCompanionType(selectedCompanion.Type);
            Debug.Log($"Companion {selectedCompanion.CompanionName} saved to GameManager with type {selectedCompanion.Type}");
        }
        else
        {
            GameManager.Instance.ClearCompanion(); // New method to clear companion
            Debug.Log("No companion selected. Cleared companion in GameManager.");
        }

        if (levelLoader != null)
        {
            levelLoader.LoadScene("LoadoutPage", "BattleScene");
        }
        else
        {
            Debug.LogError("LevelLoader reference is missing.");
        }
    }
    public void AssignCompanionToLoadout(CompanionCard companionCard)
    {
        if (companionCard == null)
        {
            Debug.LogError("Attempted to assign a null companion to the loadout.");
            return;
        }

        // Update the selected companion type in GameManager
        GameManager.Instance.SetCompanionType(companionCard.Type);

        Debug.Log($"Assigned companion {companionCard.CompanionName} to the loadout.");
    }


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
