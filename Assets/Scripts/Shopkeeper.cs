using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    public GameObject floatingText; // Text to display when the player is near
    public GameObject shopMenu;     // Reference to the shop menu UI
    public float detectionRadius = 10f; // Radius for detecting the player
    private Transform player;       // Reference to the player's transform
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody
    private Vector3 savedVelocity;  // To save player's velocity
    private bool wasKinematic;      // Track original kinematic state
    private bool isGrounded;        // Check if player is grounded before saving

    protected virtual void Start()
    {
        if (floatingText != null) floatingText.SetActive(false);
        if (shopMenu != null) shopMenu.SetActive(false);

        player = GameObject.FindWithTag("Player")?.transform;
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody == null)
                Debug.LogError("Player Rigidbody not found. Ensure the Player has a Rigidbody component.");
        }
        else
        {
            Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRadius)
        {
            HandlePlayerInRange();
        }
        else
        {
            HandlePlayerOutOfRange();
        }

        if (shopMenu != null && shopMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    private void HandlePlayerInRange()
    {
        if (floatingText != null && !floatingText.activeSelf)
        {
            floatingText.SetActive(true);
            Debug.Log("Floating text activated.");
        }

        FacePlayer();

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void HandlePlayerOutOfRange()
    {
        if (floatingText != null && floatingText.activeSelf)
        {
            floatingText.SetActive(false);
            Debug.Log("Floating text deactivated.");
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public virtual void Interact()
    {
        if (shopMenu != null && playerRigidbody != null)
        {
            if (IsPlayerGrounded())
            {
                savedVelocity = playerRigidbody.velocity; // Save only when grounded
                wasKinematic = playerRigidbody.isKinematic;
            }

            shopMenu.SetActive(true);
            playerRigidbody.velocity = Vector3.zero; // Stop movement
            playerRigidbody.isKinematic = true;      // Freeze Rigidbody
            Time.timeScale = 0f;
            Debug.Log("Shop menu opened.");
        }
    }

    public void CloseShop()
    {
        if (shopMenu != null && playerRigidbody != null)
        {
            shopMenu.SetActive(false);
            playerRigidbody.isKinematic = wasKinematic; // Restore original kinematic state
            playerRigidbody.velocity = savedVelocity;   // Restore velocity
            Time.timeScale = 1f;
            Debug.Log("Shop menu closed.");
        }
    }

    private bool IsPlayerGrounded()
    {
        return Physics.Raycast(player.position, Vector3.down, 0.2f);
    }
}
