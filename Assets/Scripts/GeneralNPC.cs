using UnityEngine;

public class GeneralNPC : MonoBehaviour
{
    public GameObject floatingText; // Text to display when the player is near
    public float detectionRadius = 10f; // Radius for detecting the player
    private Transform player; // Reference to the player's transform

    protected virtual void Start()
    {
        // Disable floating text at the start
        if (floatingText != null)
        {
            floatingText.SetActive(false);
        }

        // Locate the player by tag
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Handle interactions based on distance
        if (distanceToPlayer <= detectionRadius)
        {
            OnPlayerInRange();
        }
        else
        {
            OnPlayerOutOfRange();
        }
    }

    protected virtual void OnPlayerInRange()
    {
        // Enable floating text when in range
        if (floatingText != null && !floatingText.activeSelf)
        {
            floatingText.SetActive(true);
        }

        // Face the player
        FacePlayer();

        // Detect interaction input
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    protected virtual void OnPlayerOutOfRange()
    {
        // Disable floating text when out of range
        if (floatingText != null && floatingText.activeSelf)
        {
            floatingText.SetActive(false);
        }
    }

    private void FacePlayer()
    {
        // Make the NPC face the player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep the rotation horizontal
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public virtual void Interact()
    {
        // Default interaction behavior (can be overridden)
        Debug.Log($"{gameObject.name} interacted with.");
    }
}
