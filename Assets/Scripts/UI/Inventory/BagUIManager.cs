using UnityEngine;
using UnityEngine.UI;

public class BagUIManager : MonoBehaviour
{
    public GameObject bagUIPanel; // Reference to the Inventory UI Panel
    private bool isBagOpen = false; // Track if the inventory is open
    public Rigidbody playerRigidbody; // Reference to the player's Rigidbody
    private Vector3 savedVelocity; // To save player's velocity when paused
    private bool wasKinematic; // To save the Rigidbody's kinematic state
    public GameObject uiCanvas; // Reference to the UI_Canvas
    private PickCardsManager pickCardsManager; // Reference to the PickCardsManager script
    public Button settingsButton; // Reference to the settings button

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

        // Continuously update the settings button's state
        UpdateSettingsButtonState();
    }

    public void OnSettingsButtonClick()
    {
        // Update the current scene
        GameManager.Instance.UpdateCurrentScene();

        // Save the player state
        GameManager.Instance.SavePlayerState();

        // Find the player object by tag
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            // Get the player's current position
            Vector3 currentPlayerLocation = playerObject.transform.position;

            // Save the current player location to PlayerPrefs
            PlayerPrefs.SetFloat("templocation_x", currentPlayerLocation.x);
            PlayerPrefs.SetFloat("templocation_y", currentPlayerLocation.y);
            PlayerPrefs.SetFloat("templocation_z", currentPlayerLocation.z);

            // Save the PlayerPrefs to ensure data is persisted
            PlayerPrefs.Save();

            Debug.Log("Player location saved to PlayerPrefs.");
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }

        // Load the settings panel
        levelLoader.LoadSettingsPanel();
    }

    public void ToggleBagUI()
    {
        if (PanelManager.Instance != null)
        {
            isBagOpen = !isBagOpen; // Toggle bag state

            if (isBagOpen)
            {
                PanelManager.Instance.OpenInventory(); // Open Inventory Panel
                PausePlayerMovement();
                Time.timeScale = 0f; // Pause the game
                Debug.Log("Inventory opened.");
            }
            else
            {
                PanelManager.Instance.CloseAllPanels(); // Close all panels
                ResumePlayerMovement();
                Time.timeScale = 1f; // Resume the game
                Debug.Log("Inventory closed.");
            }
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

    public void UpdateSettingsButtonState()
    {
        if (PanelManager.Instance != null && settingsButton != null)
        {
            // Disable the settings button if inventory or shop menu is open
            bool shouldDisable = PanelManager.Instance.inventoryPanel.activeSelf ||
                                 PanelManager.Instance.shopMenuPanel.activeSelf;

            settingsButton.interactable = !shouldDisable;
        }
    }
}
