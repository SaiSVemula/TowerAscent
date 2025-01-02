using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // Ensure EventSystem namespace is imported

public class PickCardsManager : MonoBehaviour
{
    public Button pickCardsButton; // Reference to the button
    public Transform cardGrid; // Reference to the CardGrid
    public TMP_Text messageText; // Reference to the message text (for the picked cards)

    private List<GameObject> selectedCards = new List<GameObject>(); // Stores selected cards
    private bool isPickingActive = false; // Determines if picking mode is active

    private int maxCardsToPick = 4;
    private float pickingTimeout = 12f; // Timeout for card picking
    private float messageDuration = 5f; // Duration for messages to display
    private int currentMessageID = 0; // Tracks the current message ID

    void Start()
    {
        // Ensure the message text starts disabled
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }

        // Add a listener to the button
        if (SceneManager.GetActiveScene().name != "Loadout")
        {
            pickCardsButton.onClick.AddListener(ToggleCardPicking);
        }
    }

    public void ClearMessageTextOnUIOpen()
    {
        if (messageText != null)
        {
            messageText.text = ""; // Clear any lingering text
            messageText.gameObject.SetActive(false); // Ensure it is hidden
        }

        // Reset the button if UI is closed while picking is active
        if (isPickingActive)
        {
            isPickingActive = false;
            pickCardsButton.GetComponentInChildren<TMP_Text>().text = "Pick Cards for Battle";
            ResetCardHighlights();
            CancelInvoke(nameof(CancelPickingDueToTimeout));
        }
    }

    void ToggleCardPicking()
    {
        isPickingActive = !isPickingActive; // Toggle picking mode

        if (isPickingActive)
        {
            // Change button text
            pickCardsButton.GetComponentInChildren<TMP_Text>().text = "Cancel Pick";

            // Clear any previously selected cards
            ResetCardHighlights();
            selectedCards.Clear();

            // Set message text
            ShowMessage("Click on 4 cards to pick for battle.");

            // Start timeout for picking
            Invoke(nameof(CancelPickingDueToTimeout), pickingTimeout);
        }
        else
        {
            CancelPicking("Picking cancelled.");
        }
    }

    void Update()
    {
        if (isPickingActive)
        {
            if (Input.GetMouseButtonDown(0)) // Detect left mouse click
            {
                // Detect the clicked UI object
                PointerEventData pointerData =
                    new PointerEventData(EventSystem.current)
                    {
                        position = Input.mousePosition
                    };

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    GameObject clickedObject = result.gameObject;

                    // Check if the clicked object is a card
                    if (clickedObject.transform.parent == cardGrid && !selectedCards.Contains(clickedObject))
                    {
                        selectedCards.Add(clickedObject);

                        // Highlight selected card (e.g., change its color or add an outline)
                        Image cardImage = clickedObject.GetComponent<Image>();
                        if (cardImage != null)
                        {
                            cardImage.color = Color.green; // Example: Change the color to green
                        }

                        // Check if 4 cards are selected
                        if (selectedCards.Count == maxCardsToPick)
                        {
                            CompletePicking();
                        }

                        break;
                    }
                }
            }
        }
    }

    void CompletePicking()
    {
        isPickingActive = false; // Stop picking
        DisplayPickedCards(); // Show the picked cards
        pickCardsButton.GetComponentInChildren<TMP_Text>().text = "Pick Cards for Battle"; // Reset button text

        // Cancel any pending timeout
        CancelInvoke(nameof(CancelPickingDueToTimeout));
    }

    void CancelPickingDueToTimeout()
    {
        if (isPickingActive)
        {
            CancelPicking("Picking cancelled due to timeout.");
        }
    }

    void CancelPicking(string cancelMessage)
    {
        isPickingActive = false; // Stop picking
        pickCardsButton.GetComponentInChildren<TMP_Text>().text = "Pick Cards for Battle"; // Reset button text

        // Reset highlights and set cancellation message
        ResetCardHighlights();
        ShowMessage(cancelMessage);
    }

    void DisplayPickedCards()
    {
        string pickedCardsText = "Cards picked for battle: ";
        foreach (GameObject card in selectedCards)
        {
            pickedCardsText += card.name + ", ";
        }

        ShowMessage(pickedCardsText.TrimEnd(',', ' '));
    }

    void ShowMessage(string text)
    {
        if (messageText != null)
        {
            currentMessageID++; // Increment the message ID for this new message
            int messageID = currentMessageID;

            // Enable and update the message text
            messageText.gameObject.SetActive(true);
            messageText.text = text;

            // Use a coroutine for delayed message hiding
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

        // Ensure only the latest message is cleared
        if (messageID == currentMessageID && messageText != null && messageText.gameObject.activeSelf)
        {
            messageText.text = ""; // Clear the text
            messageText.gameObject.SetActive(false); // Properly hide the text
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
