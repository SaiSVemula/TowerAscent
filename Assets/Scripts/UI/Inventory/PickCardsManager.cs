using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Linq;

public class PickCardsManager : MonoBehaviour
{
    public Button pickCardsButton; // Button to enable card picking mode
    public Button saveCardPoolButton; // Button to save the selected cards
    public Transform cardGrid; // Parent object containing all card UI elements
    public TMP_Text messageText; // Text element for showing messages

    private List<GameObject> selectedCards = new List<GameObject>(); // List to track selected cards
    private bool isPickingActive = false; // Is the player currently in picking mode?

    private int maxCardsToPick = 4; // Max cards the player can select
    private float messageDuration = 5f; // How long messages are displayed for
    private int currentMessageID = 0; // Tracks the current message's ID

    private InventoryCardsRenderer inventoryCardsRenderer; // Handles rendering of the cards in the inventory UI

    void Start()
    {
        inventoryCardsRenderer = GetComponent<InventoryCardsRenderer>();

        // Initialize message text as hidden
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        // Configure button listeners
        if(SceneManager.GetActiveScene().name != "LoadoutPage")
        {
            pickCardsButton.onClick.AddListener(StartPickingMode);
            saveCardPoolButton.onClick.AddListener(SaveSelectedCards);

            // Initially hide the save button
            saveCardPoolButton.gameObject.SetActive(false);
        }
    }

    public void StartPickingMode()
    {
        isPickingActive = true;

        // Display only weapon cards for selection
        inventoryCardsRenderer.isPickingWeaponCards = true;
        inventoryCardsRenderer.RefreshInventoryUI();

        // Show the Save Card button and hide the Pick Cards button
        pickCardsButton.gameObject.SetActive(false);
        saveCardPoolButton.gameObject.SetActive(true);

        // Clear previous selections and highlights
        ResetCardHighlights();
        selectedCards.Clear();

        // Display an instructional message
        ShowMessage("Click on up to 4 weapon cards to pick for battle.");
    }

    public void SaveSelectedCards()
    {
        if (selectedCards.Count < 1)
        {
            ShowMessage("You must select at least one card to save.");
            return;
        }

        // Save selected cards to the GameManager
        SaveSelectedCardsToGameManager();

        // Exit picking mode and reset the UI
        isPickingActive = false;
        pickCardsButton.gameObject.SetActive(true);
        saveCardPoolButton.gameObject.SetActive(false);

        // Restore the full inventory view
        inventoryCardsRenderer.isPickingWeaponCards = false;
        inventoryCardsRenderer.RefreshInventoryUI();

        // Show a success message
        ShowMessage("Cards saved successfully.");
    }

    void Update()
    {
        if (isPickingActive && Input.GetMouseButtonDown(0)) // Detect mouse click
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                GameObject clickedObject = result.gameObject;

                // Check if the clicked object is a card in the card grid
                if (clickedObject.transform.parent == cardGrid && !selectedCards.Contains(clickedObject))
                {
                    selectedCards.Add(clickedObject);

                    // Highlight the selected card
                    Image cardImage = clickedObject.GetComponent<Image>();
                    if (cardImage != null)
                    {
                        cardImage.color = Color.green; // Example highlight: green color
                    }

                    // Ensure the player doesn't select more than the max allowed cards
                    if (selectedCards.Count == maxCardsToPick)
                    {
                        ShowMessage("Maximum cards selected.");
                    }

                    break;
                }
            }
        }
    }

    void SaveSelectedCardsToGameManager()
    {
        List<Card> selectedCardObjects = selectedCards
            .Select(card => card.GetComponent<CardDisplay>().GetCard())
            .ToList();

        GameManager.Instance.UpdateMiniBattleCardPool(selectedCardObjects);

        Debug.Log($"Saved {selectedCardObjects.Count} cards to MiniBattleCardPool.");
    }


    void ShowMessage(string text)
    {
        if (messageText != null)
        {
            currentMessageID++;
            int messageID = currentMessageID;

            messageText.gameObject.SetActive(true);
            messageText.text = text;

            StartCoroutine(HideMessageAfterDelay(messageID));
        }
        else
        {
            Debug.LogWarning("MessageText is not assigned in the inspector.");
        }
    }

    System.Collections.IEnumerator HideMessageAfterDelay(int messageID)
    {
        yield return new WaitForSeconds(messageDuration);

        if (messageID == currentMessageID && messageText != null && messageText.gameObject.activeSelf)
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }

    void ResetCardHighlights()
    {
        foreach (Transform card in cardGrid)
        {
            Image cardImage = card.GetComponent<Image>();
            if (cardImage != null)
            {
                cardImage.color = Color.white; // Reset color to white
            }
        }
    }
}
