using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    public GameObject floatingText; // Text to display when the player is near
    public GameObject shopMenu;     // Reference to the shop menu UI
    public float detectionRadius = 10f; // Radius for detecting the player
    private Transform player;       // Reference to the player's transform
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody

    protected virtual void Start()
    {
        if (floatingText != null) floatingText.SetActive(false);
        if (shopMenu != null) shopMenu.SetActive(false);

        player = GameObject.FindWithTag("Player")?.transform;
        if (player != null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsPlayerGrounded())
            {
                Interact();
            }
            else
            {
                Debug.Log("Cannot open shop while jumping.");
            }
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

    public virtual void Interact()
    {
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.OpenShopMenu(); // Open Shop Menu Panel
            Time.timeScale = 0f;
            Debug.Log("Shop menu opened.");
        }
    }

    public void CloseShop()
    {
        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.CloseAllPanels(); // Close all panels
            Time.timeScale = 1f;
            Debug.Log("Shop menu closed.");
        }
    }

    private bool IsPlayerGrounded()
    {
        // Check if the player's Rigidbody is grounded by evaluating the vertical velocity
        return playerRigidbody != null && Mathf.Abs(playerRigidbody.velocity.y) < 0.01f;
    }
}