using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool IsOccupied => transform.childCount > 0; // Check if the slot has an item

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        if (droppedItem == null) return;

        DraggableItem draggableItem = droppedItem.GetComponent<DraggableItem>();
        if (draggableItem == null) return;

        // Only allow dropping if the slot is not already occupied
        if (!IsOccupied)
        {
            draggableItem.parentAfterDrag = transform;
            Debug.Log($"Item {droppedItem.name} successfully dropped into slot {name}");
        }
        else
        {
            Debug.LogWarning($"Slot {name} is already occupied. Item will return to inventory.");
        }
    }
}
