using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    public GameObject floatingText; // Text to display when the player is near
    public GameObject shopMenu;     // Reference to the shop menu UI
    public float detectionRadius = 10f; // Radius for detecting the player
    public Rigidbody playerRigidbody; // Reference to the player's Rigidbody (assign in inspector)
    private Vector3 savedVelocity; // To save player's velocity when paused
    private bool wasKinematic; // To save the Rigidbody's kinematic state
    private bool isShopOpen = false; // Track if the shop is open

    protected virtual void Start()
    {
        if (floatingText != null) floatingText.SetActive(false);
        if (shopMenu != null) shopMenu.SetActive(false);

        if (playerRigidbody == null)
        {
            Debug.LogError("Player Rigidbody not assigned in the inspector.");
        }
    }

    void Update()
    {
        if (playerRigidbody == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerRigidbody.transform.position);
        if (distanceToPlayer <= detectionRadius)
        {
            HandlePlayerInRange();
        }
        else
        {
            HandlePlayerOutOfRange();
        }

        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void HandlePlayerInRange()
    {
        if (floatingText != null && !floatingText.activeSelf)
        {
            floatingText.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.E) && !isShopOpen)
        {
            OpenShop();
        }
    }

    private void HandlePlayerOutOfRange()
    {
        if (floatingText != null && floatingText.activeSelf)
        {
            floatingText.SetActive(false);
        }
    }

    private void OpenShop()
    {
        if (PanelManager.Instance != null && shopMenu != null)
        {
            // Prevent opening Shop if Bag is already open
            if (PanelManager.Instance.IsPlayerMovementPaused() && !isShopOpen)
            {
                Debug.Log("Cannot open Shop while another menu is open.");
                return;
            }

            shopMenu.SetActive(true); // Open Shop Menu
            isShopOpen = true;
            PanelManager.Instance.PausePlayerMovement();
            PausePlayerMovement();
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Shop menu opened.");
        }
    }

    private void CloseShop()
    {
        if (PanelManager.Instance != null && shopMenu != null)
        {
            shopMenu.SetActive(false); // Close Shop Menu
            isShopOpen = false;
            PanelManager.Instance.CloseAllPanels(); // Notify PanelManager
            ResumePlayerMovement();
            Time.timeScale = 1f; // Resume the game
            Debug.Log("Shop menu closed.");
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
            if (!wasKinematic) // Only restore velocity if the Rigidbody was not kinematic
            {
                playerRigidbody.velocity = savedVelocity; // Restore previous velocity
            }
        }
    }
}
