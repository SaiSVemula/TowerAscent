using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutTabsManager : MonoBehaviour
{
    [SerializeField] private Button cardsButton; // Button to show cards
    [SerializeField] private Button companionsButton; // Button to show companions
    [SerializeField] private GameObject cardGrid; // Reference to the cards grid
    [SerializeField] private GameObject companionGrid; // Reference to the companions grid
    [SerializeField] private InventoryCardsRenderer cardsRenderer; // Card renderer
    [SerializeField] private LoadoutCompanionsRenderer companionsRenderer; // Companion renderer
    [SerializeField] private TextMeshProUGUI noItemsText; // Text to display when no items are present

    private Color activeColor = Color.white; // Highlight color for the active button
    private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f); // Greyed-out color for inactive button

    private void Start()
    {
        // Assign button click events
        cardsButton.onClick.AddListener(() => ShowTab("Cards"));
        companionsButton.onClick.AddListener(() => ShowTab("Companions"));

        // Default to showing cards
        ShowTab("Cards");
    }

    public void ShowTab(string tabName)
    {
        noItemsText.gameObject.SetActive(false);

        if (tabName == "Cards")
        {
            SetButtonState(cardsButton, true);
            SetButtonState(companionsButton, false);

            cardGrid.SetActive(true); // Show cards grid
            companionGrid.SetActive(false); // Hide companions grid

            // Refresh cards UI (only if needed)
            cardsRenderer.RefreshInventoryUI();
        }
        else if (tabName == "Companions")
        {
            SetButtonState(cardsButton, false);
            SetButtonState(companionsButton, true);

            cardGrid.SetActive(false); // Hide cards grid
            companionGrid.SetActive(true); // Show companions grid

            var ownedCompanions = GameManager.Instance?.GetOwnedCompanions() ?? new System.Collections.Generic.List<CompanionCard>();
            if (ownedCompanions.Count == 0)
            {
                DisplayNoItemsMessage("No companions available.");
                return;
            }

            noItemsText.gameObject.SetActive(false); // Hide the no-items text
            companionsRenderer.RenderCompanions(ownedCompanions, companionGrid.transform); // Render companions only once
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

    private void DisplayNoItemsMessage(string message)
    {
        noItemsText.text = message;
        noItemsText.gameObject.SetActive(true);
    }
}
