using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using TMPro;

public class LoadoutSlot : MonoBehaviour, IDropHandler
{
    public event Action OnCardChanged; // Event to notify changes in the slot

    [SerializeField] public string slotType; // Expected type for the slot
    [SerializeField] private Transform inventoryGrid; // Reference to the inventory grid
    [SerializeField] private TextMeshProUGUI slotText; // Text to display the slot type

    public bool IsOccupied => transform.childCount > 0; // Check if the slot is occupied

    private bool ValidateCardType(object card)
    {
        switch (slotType)
        {
            case "WeaponCard":
                return card is WeaponCard;
            case "MagicCard":
                return card is MagicCard;
            case "DefenceCard":
                return card is DefenceCard;
            case "HealingCard":
                return card is HealingCard;
            case "CombinationCard":
                return card is CombinationCard;
            case "CompanionCard": // Add support for CompanionCard
                return card is CompanionCard;
            default:
                Debug.LogError($"Invalid slot type: {slotType}");
                return false;
        }
    }

    // Check if the card is valid for the slot
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null)
        {
            Debug.LogWarning("No item was dragged onto the slot.");
            return;
        }

        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
        if (draggableItem == null)
        {
            Debug.LogWarning("Dragged item is not draggable.");
            return;
        }

        if (slotType == "CompanionCard")
        {
            CompanionCardDisplay companionCardDisplay = droppedItem.GetComponent<CompanionCardDisplay>();
            if (companionCardDisplay == null || companionCardDisplay.CompanionCardData == null)
            {
                Debug.LogWarning("Dragged item has no valid companion card data. Returning to inventory.");
                ReturnCardToInventory(droppedItem);
                return;
            }

            if (!IsOccupied)
            {
                draggableItem.parentAfterDrag = transform;
                SetCardTextColor(Color.green); // Update slot text to green
                GameManager.Instance.AddCompanion(companionCardDisplay.CompanionCardData); // Save the companion
                Debug.Log($"Companion {companionCardDisplay.CompanionCardData.CompanionName} added to the slot.");
            }
            else
            {
                Debug.LogWarning($"Slot {slotType} is occupied. Returning card to inventory.");
                ReturnCardToInventory(droppedItem);
            }
        }
        else
        {
            CardDisplay cardDisplay = droppedItem.GetComponent<CardDisplay>();
            if (cardDisplay == null || cardDisplay.CardData == null)
            {
                Debug.LogWarning("Dragged item has no valid card data. Returning to inventory.");
                ReturnCardToInventory(droppedItem);
                return;
            }

            if (ValidateCardType(cardDisplay.CardData))
            {
                if (!IsOccupied)
                {
                    draggableItem.parentAfterDrag = transform;
                    SetCardTextColor(Color.green); // Update slot text to green
                    Debug.Log($"Card {cardDisplay.CardData.Name} added to {slotType} slot.");
                }
                else
                {
                    Debug.LogWarning($"Slot {slotType} is occupied. Returning card to inventory.");
                    ReturnCardToInventory(droppedItem);
                }
            }
            else
            {
                Debug.LogWarning($"Card {cardDisplay.CardData.Name} is invalid for {slotType}. Returning to inventory.");
                ReturnCardToInventory(droppedItem);
            }
        }
    }


    //public bool IsValidCard()
    //{
    //    if (IsOccupied)
    //    {
    //        CardDisplay cardDisplay = transform.GetChild(0).GetComponent<CardDisplay>();
    //        if (cardDisplay != null)
    //        {
    //            bool isValid = ValidateCardType(cardDisplay.CardData);
    //            Debug.Log($"Validating card {cardDisplay.CardData.Name} for slot {slotType}. IsValid: {isValid}");
    //            return isValid;
    //        }
    //    }
    //    Debug.Log($"Slot {slotType} is empty or invalid.");
    //    return false;
    //}

    private void ReturnCardToInventory(GameObject cardObject)
    {
        cardObject.transform.SetParent(inventoryGrid);
        cardObject.transform.localPosition = Vector3.zero;
        Debug.Log($"Card returned to inventory.");
        SetCardTextColor(Color.black); // Passing black
    }

    public void SetCardTextColor(Color color)
    {
        if (slotText != null)
        {
            slotText.color = color; // Set the text color
        }
        else
        {
            Debug.LogWarning("No Text component found on the card.");
        }
    }
}

