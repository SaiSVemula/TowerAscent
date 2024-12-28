using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag; // Target parent after dragging
    private Transform originalParent; // Original parent before dragging
    private Vector3 originalPosition; // Original position before dragging

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");

        originalParent = transform.parent; // Save the original parent
        originalPosition = transform.localPosition; // Save the original position
        parentAfterDrag = originalParent; // Default to the original parent

        transform.SetParent(transform.parent.parent); // Detach from the parent (Grid Layout)
        transform.SetAsLastSibling(); // Bring to the front
        image.raycastTarget = false; // Allow raycast to pass through
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition; // Follow the mouse position
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");

        // If dropped outside a valid slot, return to the original parent and position
        if (transform.parent == originalParent.parent)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition; // Reset to the original position
            Debug.Log("Item returned to original inventory position.");
        }

        // If dropped in a valid slot, update parentAfterDrag
        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero; // Center in the slot
        image.raycastTarget = true; // Re-enable raycast
    }
}
