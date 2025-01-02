using UnityEngine;
using TMPro;

public class TextHighlighter : MonoBehaviour
{
    [Header("Slot References")]
    [SerializeField] private InventorySlot weaponSlot;
    [SerializeField] private InventorySlot magicSlot;
    [SerializeField] private InventorySlot defenceSlot;
    [SerializeField] private InventorySlot healingSlot;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI weaponSlotText;
    [SerializeField] private TextMeshProUGUI magicSlotText;
    [SerializeField] private TextMeshProUGUI defenceSlotText;
    [SerializeField] private TextMeshProUGUI healingSlotText;

    [Header("Colours For Text")]
    [SerializeField] private Color validCardColor = Color.white; // Full white for valid card
    [SerializeField] private Color defaultColor = Color.gray;   // Grey for invalid or empty slot

    private void Start()
    {
        ResetSlotColours();

        // Subscribe to slot updates
        SubscribeToSlotEvents();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        UnsubscribeFromSlotEvents();
    }

    private void SubscribeToSlotEvents()
    {
        if (weaponSlot != null) weaponSlot.OnCardChanged += () => UpdateSlotTextColor(weaponSlot, weaponSlotText);
        if (magicSlot != null) magicSlot.OnCardChanged += () => UpdateSlotTextColor(magicSlot, magicSlotText);
        if (defenceSlot != null) defenceSlot.OnCardChanged += () => UpdateSlotTextColor(defenceSlot, defenceSlotText);
        if (healingSlot != null) healingSlot.OnCardChanged += () => UpdateSlotTextColor(healingSlot, healingSlotText);
    }


    private void UnsubscribeFromSlotEvents()
    {
        weaponSlot.OnCardChanged -= () => UpdateSlotTextColor(weaponSlot, weaponSlotText);
        magicSlot.OnCardChanged -= () => UpdateSlotTextColor(magicSlot, magicSlotText);
        defenceSlot.OnCardChanged -= () => UpdateSlotTextColor(defenceSlot, defenceSlotText);
        healingSlot.OnCardChanged -= () => UpdateSlotTextColor(healingSlot, healingSlotText);
    }

    private void UpdateSlotTextColor(InventorySlot slot, TextMeshProUGUI slotText)
    {
        if (slot.IsValidCard())
        {
            slotText.color = validCardColor; // Valid card in slot
            Debug.Log($"Slot text for {slot.name} updated to valid color.");
        }
        else
        {
            slotText.color = defaultColor; // No valid card in slot
            Debug.Log($"Slot text for {slot.name} updated to default color.");
        }
    }

    private void ResetSlotColours()
    {
        weaponSlotText.color = defaultColor;
        magicSlotText.color = defaultColor;
        defenceSlotText.color = defaultColor;
        healingSlotText.color = defaultColor;
    }
}
