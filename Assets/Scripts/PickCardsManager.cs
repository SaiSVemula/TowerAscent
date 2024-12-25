using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickCardsManager : MonoBehaviour
{
    public Button pickCardsButton; // Reference to the button
    public Transform cardGrid; // Reference to the CardGrid
    public TMP_Text messageText; // Reference to the message text (for the picked cards)

    private List<GameObject> selectedCards = new List<GameObject>(); // Stores selected cards
    private bool isPickingActive = false; // Determines if picking mode is active

    private int maxCardsToPick = 4;
    private Coroutine messageCoroutine; // For handling message timeout

    void Start()
    {
        // Add a listener to the button
        pickCardsButton.onClick.AddListener(ToggleCardPicking);
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
            SetMessage("Click on 4 cards to pick for battle.");
        }
        else
        {
            // Change button text back
            pickCardsButton.GetComponentInChildren<TMP_Text>().text = "Pick Cards for Battle";

            // Set cancelled message
            SetMessage("Picking cancelled.");
        }
    }

    void Update()
    {
        if (isPickingActive)
        {
            if (Input.GetMouseButtonDown(0)) // Detect left mouse click
            {
                // Detect the clicked UI object
                UnityEngine.EventSystems.PointerEventData pointerData =
                    new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
                    {
                        position = Input.mousePosition
                    };

                List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);

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
                            isPickingActive = false; // Stop picking
                            DisplayPickedCards(); // Show the picked cards
                            pickCardsButton.GetComponentInChildren<TMP_Text>().text = "Pick Cards for Battle"; // Reset button text
                        }

                        break;
                    }
                }
            }
        }
    }

    void DisplayPickedCards()
    {
        string pickedCardsText = "Cards picked for battle: ";
        foreach (GameObject card in selectedCards)
        {
            pickedCardsText += card.name + ", ";
        }

        SetMessage(pickedCardsText.TrimEnd(',', ' ')); // Update message text
    }

    void SetMessage(string text)
    {
        // Stop any existing coroutine
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        // Update message text
        messageText.text = text;

        // Start coroutine to clear the message after 5 seconds
        messageCoroutine = StartCoroutine(ClearMessageAfterDelay(5f));
    }

    System.Collections.IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Check if the message being cleared is still the current message
        if (messageText.text != "")
        {
            messageText.text = "";
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
