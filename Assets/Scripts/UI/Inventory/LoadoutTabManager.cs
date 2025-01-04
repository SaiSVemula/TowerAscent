using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutTabsManager : MonoBehaviour
{
    [SerializeField] private Button cardsButton; // Button to show cards
    [SerializeField] private Button companionsButton; // Button to show companions
    [SerializeField] private Transform grid; // Shared grid for displaying cards and companions
    [SerializeField] private InventoryCardsRenderer cardsRenderer; // Card renderer
    [SerializeField] private LoadoutCompanionsRenderer companionsRenderer; // Companion renderer
    [SerializeField] private TextMeshProUGUI noItemsText; // Text to display when no items are present

    private Color activeColor = Color.white; // Highlight color for the active button
    private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f); // Greyed-out color for inactive button

    private void Start()
    {
        //// Assign button click events
        //cardsButton.onClick.AddListener(() => ShowTab("Cards"));
        //companionsButton.onClick.AddListener(() => ShowTab("Companions"));

        // Default to showing cards
        ShowTab("Cards");
    }

    public void ShowTab(string tabName)
    {
        ClearGrid(); // Clear the current grid content

        if (tabName == "Cards")
        {
            SetButtonState(cardsButton, true);
            SetButtonState(companionsButton, false);

            // Use the existing RefreshInventoryUI method for cards
            cardsRenderer.RefreshInventoryUI();
        }
        else if (tabName == "Companions")
        {
            SetButtonState(cardsButton, false);
            SetButtonState(companionsButton, true);

            var ownedCompanions = GameManager.Instance?.GetOwnedCompanions() ?? new System.Collections.Generic.List<CompanionCard>();
            if (ownedCompanions.Count == 0)
            {
                DisplayNoItemsMessage("No companions available.");
                return;
            }

            noItemsText.gameObject.SetActive(false); // Hide the no-items text
            companionsRenderer.RenderCompanions(ownedCompanions, grid); // Render companions in the shared grid
        }
    }

    private void SetButtonState(Button button, bool isActive)
    {
        // Change the button color based on active state
        var buttonText = button.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.color = isActive ? activeColor : inactiveColor;
        }
    }

    private void ClearGrid()
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
    }

    private void DisplayNoItemsMessage(string message)
    {
        ClearGrid(); // Ensure grid is cleared
        noItemsText.text = message;
        noItemsText.gameObject.SetActive(true);
    }
}
