using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import for UI components

public class NotesInteraction : MonoBehaviour
{
    public GameObject floatingText; // The floating "Press E to interact" text
    public GameObject noteUI;       // The UI that shows the note content
    public Text noteText;           // The Text component in the UI to display the note
    private bool playerNearby = false; // Tracks if the player is near the note

    // Start is called before the first frame update
    void Start()
    {
        floatingText.SetActive(false); // Hide floating text initially
        noteUI.SetActive(false);      // Hide the note UI initially
    }

    // What to do when the player is near
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the object entering is the player
        {
            playerNearby = true;       // Set the playerNearby flag
            floatingText.SetActive(true); // Show floating text
        }
    }

    // What to do when the player is not near
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the object exiting is the player
        {
            playerNearby = false;      // Reset the playerNearby flag
            floatingText.SetActive(false); // Hide floating text
            noteUI.SetActive(false);   // Hide the note UI when leaving
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is nearby and presses E
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Toggle the note UI visibility
            noteUI.SetActive(!noteUI.activeSelf);

            // Hide floating text when the note UI is active
            if (noteUI.activeSelf)
            {
                floatingText.SetActive(false);
                // Set the text for the note (example)
                noteText.text = "This is the content of the note. Replace with your text.";
            }
            else
            {
                floatingText.SetActive(true);
            }
        }
    }
}
