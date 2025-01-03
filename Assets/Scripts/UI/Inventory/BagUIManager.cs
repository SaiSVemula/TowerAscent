using UnityEngine;

public class BagUIManager : MonoBehaviour
{
    public GameObject bagUIPanel; // Reference to the Inventory UI Panel
    private bool isBagOpen = false; // Track if the inventory is open
    public Rigidbody playerRigidbody; // Reference to the player's Rigidbody
    private Vector3 savedVelocity; // To save player's velocity when paused
    private bool wasKinematic; // To save the Rigidbody's kinematic state
    public GameObject uiCanvas; // Reference to the UI_Canvas
    private PickCardsManager pickCardsManager; // Reference to the PickCardsManager script

    private LevelLoader levelLoader;

    void Start()
    {
        // Find the LevelLoader instance in the current scene
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }

        // Automatically disable and enable the canvas to fix button issues
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(false); // Temporarily disable the canvas
            uiCanvas.SetActive(true); // Re-enable it to refresh EventSystem
        }
        else
        {
            Debug.LogWarning("UI_Canvas not assigned in the inspector.");
        }

        // Automatically grab reference to PickCardsManager
        pickCardsManager = GetComponent<PickCardsManager>();
        if (pickCardsManager == null)
        {
            Debug.LogWarning("PickCardsManager not found on the same GameObject!");
        }
    }

    void Update()
    {
        // Check if the "I" key is pressed to toggle the bag
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleBagUI();
        }
    }

    public void OnSettingsButtonClick()
    {
        levelLoader.LoadSettingsPanel();
    }

    // Method to toggle the bag UI visibility
    public void ToggleBagUI()
    {
        isBagOpen = !isBagOpen; // Invert the state
        bagUIPanel.SetActive(isBagOpen);

        if (isBagOpen)
        {
            PausePlayerMovement(); // Freeze the player when inventory opens
            Time.timeScale = 0f;   // Pause the game

            // Clear the message text if the UI is opened
            //pickCardsManager.ClearMessageTextOnUIOpen();

            Debug.Log("Inventory opened.");
        }
        else
        {
            ResumePlayerMovement(); // Resume movement when inventory closes
            Time.timeScale = 1f;    // Resume the game
            Debug.Log("Inventory closed.");
        }
    }

    private void PausePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            savedVelocity = playerRigidbody.velocity; // Save current velocity
            wasKinematic = playerRigidbody.isKinematic; // Save kinematic state

            playerRigidbody.velocity = Vector3.zero; // Stop the Rigidbody
            playerRigidbody.isKinematic = true; // Temporarily freeze the Rigidbody
        }
        else
        {
            Debug.LogWarning("Player Rigidbody not assigned in the inspector.");
        }
    }

    private void ResumePlayerMovement()
    {
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = wasKinematic; // Restore original kinematic state
            playerRigidbody.velocity = savedVelocity; // Restore previous velocity
        }
    }

    // Bind this method to the Bag Button's OnClick() in the Inspector
    public void OnBagButtonClicked()
    {
        ToggleBagUI();
    }
}
