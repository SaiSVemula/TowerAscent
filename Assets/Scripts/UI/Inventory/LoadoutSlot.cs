using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class LoadoutSlot : MonoBehaviour, IDropHandler
{
    public event Action OnCardChanged; // Event to notify changes in the slot

    [SerializeField] public string slotType; // Expected type for the slot
    [SerializeField] private Transform inventoryGrid; // Reference to the inventory grid

    public bool IsOccupied => transform.childCount > 0; // Check if the slot is occupied

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
                StartCoroutine(DelayedNotify());
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

    public bool IsValidCard()
    {
        if (IsOccupied)
        {
            CardDisplay cardDisplay = transform.GetChild(0).GetComponent<CardDisplay>();
            if (cardDisplay != null)
            {
                bool isValid = ValidateCardType(cardDisplay.CardData);
                Debug.Log($"Validating card {cardDisplay.CardData.Name} for slot {slotType}. IsValid: {isValid}");
                return isValid;
            }
        }
        Debug.Log($"Slot {slotType} is empty or invalid.");
        return false;
    }


    private bool ValidateCardType(Card card)
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
            default:
                Debug.LogError($"Invalid slot type: {slotType}");
                return false;
        }
    }

    private void ReturnCardToInventory(GameObject cardObject)
    {
        cardObject.transform.SetParent(inventoryGrid);
        cardObject.transform.localPosition = Vector3.zero;
        Debug.Log($"Card returned to inventory.");
        StartCoroutine(DelayedNotify());
    }
    private IEnumerator DelayedNotify()
    {
        yield return null; // Wait for the next frame
        Debug.Log($"Card change event triggered for slot {slotType} after delay.");
        OnCardChanged?.Invoke();
    }
}

