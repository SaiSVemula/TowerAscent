using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    public GameObject floatingText; // Text to display when the player is near
    public GameObject shopMenu; // Reference to the shop menu UI
    public float detectionRadius = 10f; // Radius for detecting the player
    private Transform player; // Reference to the player's transform

    protected virtual void Start()
    {
        // Ensure floating text is disabled at the start
        if (floatingText != null)
        {
            floatingText.SetActive(false);
        }

        // Ensure shop menu is disabled at the start
        if (shopMenu != null)
        {
            shopMenu.SetActive(false);
        }

        // Find the player using the Player tag
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calculate the distance between the player and the NPC
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            HandlePlayerInRange();
        }
        else
        {
            HandlePlayerOutOfRange();
        }

        // Allow exiting the shop menu with Escape
        if (shopMenu != null && shopMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void HandlePlayerInRange()
    {
        // Show floating text if the player is in range
        if (floatingText != null && !floatingText.activeSelf)
        {
            floatingText.SetActive(true);
            Debug.Log("Floating text activated.");
        }

        // Make the NPC face the player
        FacePlayer();

        // Check for interaction input
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed. Interact called.");
            Interact();
        }
    }

    private void HandlePlayerOutOfRange()
    {
        // Hide floating text if the player moves out of range
        if (floatingText != null && floatingText.activeSelf)
        {
            floatingText.SetActive(false);
            Debug.Log("Floating text deactivated.");
        }
    }

    private void FacePlayer()
    {
        // Rotate the NPC to face the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep the rotation horizontal
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Shopkeeper Interact method triggered.");

        if (shopMenu != null)
        {
            Debug.Log($"Shop menu reference is valid. Active state before activation: {shopMenu.activeSelf}");
            shopMenu.SetActive(true); // Show the shop menu
            Debug.Log($"Shop menu active state after activation: {shopMenu.activeSelf}");
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Shop menu opened.");
        }
        else
        {
            Debug.LogError("Shop menu is null. Please assign it in the Inspector.");
        }
    }

    public void CloseShop()
    {
        if (shopMenu != null)
        {
            Debug.Log("Closing shop menu.");
            shopMenu.SetActive(false); // Hide the shop menu
            Time.timeScale = 1f; // Resume the game
        }
        else
        {
            Debug.LogError("Shop menu reference is missing. Cannot close shop.");
        }
    }
}
