//using UnityEngine;
//using UnityEngine.EventSystems;

//public class InventorySlot : MonoBehaviour, IDropHandler
//{
//    public bool IsOccupied => transform.childCount > 0; // Check if the slot has an item

//    public void OnDrop(PointerEventData eventData)
//    {
//        GameObject droppedItem = eventData.pointerDrag;
//        if (droppedItem == null) return;

//        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
//        if (draggableItem == null) return;

//        // Only allow dropping if the slot is not already occupied
//        if (!IsOccupied)
//        {
//            draggableItem.parentAfterDrag = transform;
//            Debug.Log($"Item {droppedItem.name} successfully dropped into slot {name}");
//        }
//        else
//        {
//            Debug.LogWarning($"Slot {name} is already occupied. Item will return to inventory.");
//        }
//    }
//}

//using UnityEngine;
//using UnityEngine.EventSystems;

//public class InventorySlot : MonoBehaviour, IDropHandler
//{
//    public bool IsOccupied => transform.childCount > 0; // Check if the slot has an item

//    public void OnDrop(PointerEventData eventData)
//    {
//        GameObject droppedItem = eventData.pointerDrag;
//        if (droppedItem == null) return;

//        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
//        if (draggableItem == null) return;

//        // Check if the slot is occupied
//        if (!IsOccupied)
//        {
//            draggableItem.parentAfterDrag = transform; // Update the target parent
//            Debug.Log($"Item {droppedItem.name} successfully dropped into slot {name}");
//        }
//        else
//        {
//            Debug.LogWarning($"Slot {name} is already occupied. Item will return to inventory.");
//        }
//    }
//}


using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private string slotType; // Define the expected type for this slot ("Weapon", "Magic", "Defence", "Healing")
    [SerializeField] private Transform inventoryGrid; // Reference to the inventory grid

    public bool IsOccupied => transform.childCount > 0; // Check if the slot has an item

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
        if (draggableItem == null) return;

        // Get the Card reference from the CardDisplay component
        CardDisplay cardDisplay = droppedItem.GetComponent<CardDisplay>();
        if (cardDisplay == null || cardDisplay.CardData == null)
        {
            Debug.LogWarning("Dropped item does not contain a valid CardDisplay or CardData. Returning to inventory.");
            ReturnCardToInventory(droppedItem);
            return;
        }

        Card card = cardDisplay.CardData; // Get the card data

        // Check if the card matches the expected type for this slot
        bool isValid = ValidateCardType(card);

        if (isValid)
        {
            if (!IsOccupied)
            {
                draggableItem.parentAfterDrag = transform; // Attach the card to this slot
                Debug.Log($"Card {card.Name} successfully placed in slot {slotType}.");
            }
            else
            {
                Debug.LogWarning($"Slot {name} is already occupied. Returning {card.Name} to inventory.");
                ReturnCardToInventory(droppedItem);
            }
        }
        else
        {
            Debug.LogWarning($"Card {card.Name} is not valid for slot {slotType}. Returning to inventory.");
            ReturnCardToInventory(droppedItem);
        }
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
                Debug.LogWarning($"Invalid slot type: {slotType}. Returning card to inventory.");
                return false;
        }
    }

    private void ReturnCardToInventory(GameObject cardObject)
    {
        cardObject.transform.SetParent(inventoryGrid);
        cardObject.transform.localPosition = Vector3.zero; // Reset position in the grid
        Debug.Log($"Card {cardObject.name} returned to inventory.");
    }
}
