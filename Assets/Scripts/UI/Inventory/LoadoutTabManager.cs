using UnityEngine;
using UnityEngine.UI;

public class LoadoutTabsManager : MonoBehaviour
{
    [SerializeField] private Button cardsButton; // Button to display cards
    [SerializeField] private Button companionsButton; // Button to display companions
    [SerializeField] private InventoryCardsRenderer cardsRenderer; // Reference to the cards renderer
    [SerializeField] private InventoryCompanionsRenderer companionsRenderer; // Reference to the companions renderer

    private void Start()
    {
        // Assign button click events
        cardsButton.onClick.AddListener(ShowCardsTab);
        companionsButton.onClick.AddListener(ShowCompanionsTab);

        // Default to showing cards
        ShowCardsTab();
    }

    private void ShowCardsTab()
    {
        cardsRenderer.gameObject.SetActive(true);
        companionsRenderer.gameObject.SetActive(false);
    }

    private void ShowCompanionsTab()
    {
        cardsRenderer.gameObject.SetActive(false);
        companionsRenderer.gameObject.SetActive(true);
    }
}
