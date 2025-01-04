using UnityEngine;
using TMPro;

public class TextHighlighter : MonoBehaviour
{
    [Header("Slot References")]
    [SerializeField] private LoadoutSlot weaponSlot;
    [SerializeField] private LoadoutSlot magicSlot;
    [SerializeField] private LoadoutSlot defenceSlot;
    [SerializeField] private LoadoutSlot healingSlot;

    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI weaponSlotText;
    [SerializeField] private TextMeshProUGUI magicSlotText;
    [SerializeField] private TextMeshProUGUI defenceSlotText;
    [SerializeField] private TextMeshProUGUI healingSlotText;

    [Header("Colours For Text")]
    [SerializeField] private Color validCardColor = Color.green; // Full white for valid card
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
        if (weaponSlot != null) weaponSlot.OnCardChanged -= () => UpdateSlotTextColor(weaponSlot, weaponSlotText);
        if (magicSlot != null) magicSlot.OnCardChanged -= () => UpdateSlotTextColor(magicSlot, magicSlotText);
        if (defenceSlot != null) defenceSlot.OnCardChanged -= () => UpdateSlotTextColor(defenceSlot, defenceSlotText);
        if (healingSlot != null) healingSlot.OnCardChanged -= () => UpdateSlotTextColor(healingSlot, healingSlotText);
    }

    private void UpdateSlotTextColor(LoadoutSlot slot, TextMeshProUGUI slotText)
    {
        Debug.Log($"Updating text color for slot {slot.slotType}. IsValidCard: {slot.IsValidCard()}");

        if (slot.IsValidCard())
        {
            slotText.color = validCardColor;
            Debug.Log($"Slot {slot.slotType} text color set to valid (green).");
        }
        else
        {
            slotText.color = defaultColor;
            Debug.Log($"Slot {slot.slotType} text color set to default (gray).");
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
