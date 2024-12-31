//using UnityEngine;
//using UnityEngine.EventSystems;

//public class InventorySlot : MonoBehaviour, IDropHandler
//{
//    [SerializeField] private string slotType; // Define the expected type for this slot ("Weapon", "Magic", "Defence", "Healing")
//    [SerializeField] private Transform inventoryGrid; // Reference to the inventory grid

//    public bool IsOccupied => transform.childCount > 0; // Check if the slot has an item

//    public void OnDrop(PointerEventData eventData)
//    {
//        GameObject droppedItem = eventData.pointerDrag;
//        if (droppedItem == null) return;

//        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
//        if (draggableItem == null) return;

//        // Get the Card reference from the CardDisplay component
//        CardDisplay cardDisplay = droppedItem.GetComponent<CardDisplay>();
//        if (cardDisplay == null || cardDisplay.CardData == null)
//        {
//            Debug.LogWarning("Dropped item does not contain a valid CardDisplay or CardData. Returning to inventory.");
//            ReturnCardToInventory(droppedItem);
//            return;
//        }

//        Card card = cardDisplay.CardData; // Get the card data

//        // Check if the card matches the expected type for this slot
//        bool isValid = ValidateCardType(card);

//        if (isValid)
//        {
//            if (!IsOccupied)
//            {
//                draggableItem.parentAfterDrag = transform; // Attach the card to this slot
//                Debug.Log($"Card {card.Name} successfully placed in slot {slotType}.");
//            }
//            else
//            {
//                Debug.LogWarning($"Slot {name} is already occupied. Returning {card.Name} to inventory.");
//                ReturnCardToInventory(droppedItem);
//            }
//        }
//        else
//        {
//            Debug.LogWarning($"Card {card.Name} is not valid for slot {slotType}. Returning to inventory.");
//            ReturnCardToInventory(droppedItem);
//        }
//    }

//    private bool ValidateCardType(Card card)
//    {
//        switch (slotType)
//        {
//            case "WeaponCard":
//                return card is WeaponCard;
//            case "MagicCard":
//                return card is MagicCard;
//            case "DefenceCard":
//                return card is DefenceCard;
//            case "HealingCard":
//                return card is HealingCard;
//            default:
//                Debug.LogWarning($"Invalid slot type: {slotType}. Returning card to inventory.");
//                return false;
//        }
//    }

//    private void ReturnCardToInventory(GameObject cardObject)
//    {
//        cardObject.transform.SetParent(inventoryGrid);
//        cardObject.transform.localPosition = Vector3.zero; // Reset position in the grid
//        Debug.Log($"Card {cardObject.name} returned to inventory.");
//    }
//}


using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public event Action OnCardChanged; // Event to notify changes in the slot

    [SerializeField] private string slotType; // Expected type for the slot
    [SerializeField] private Transform inventoryGrid; // Reference to the inventory grid

    public bool IsOccupied => transform.childCount > 0; // Check if the slot is occupied

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
        if (draggableItem == null) return;

        CardDisplay cardDisplay = droppedItem.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.CardData == null)
        {
            Debug.LogWarning("Dropped item has no valid card data. Returning to inventory.");
            ReturnCardToInventory(droppedItem);
            NotifyCardChange();
            return;
        }

        if (ValidateCardType(cardDisplay.CardData))
        {
            if (!IsOccupied)
            {
                draggableItem.parentAfterDrag = transform;
                Debug.Log($"Card {cardDisplay.CardData.Name} added to {slotType} slot.");
                NotifyCardChange();
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
                Debug.Log($"Validating card {cardDisplay.CardData.Name} for slot {slotType}.");
                return ValidateCardType(cardDisplay.CardData);
            }
        }
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
        NotifyCardChange();
    }

    private void NotifyCardChange()
    {
        Debug.Log($"Card change event triggered for slot {slotType}.");
        OnCardChanged?.Invoke();
    }
}

