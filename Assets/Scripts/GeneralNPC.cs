using UnityEngine;

public class GeneralNPC : MonoBehaviour
{
    public GameObject floatingText; // Text to display when the player is near
    public float detectionRadius = 3f; // Radius for detecting the player
    private Transform player; // Reference to the player's transform

    protected virtual void Start() // Use "protected virtual" for inheritance
    {
        // Hide floating text by default
        if (floatingText != null)
        {
            floatingText.SetActive(false);
        }

        // Find the player by tag
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the Player GameObject has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // Check the distance between the NPC and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            HandlePlayerInRange();
        }
        else
        {
            HandlePlayerOutOfRange();
        }
    }

    private void HandlePlayerInRange()
    {
        // Show floating text and face the player
        if (floatingText != null && !floatingText.activeSelf)
        {
            floatingText.SetActive(true);
        }

        FacePlayer();

        // Handle interaction if the player presses 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void HandlePlayerOutOfRange()
    {
        // Hide floating text when the player moves out of range
        if (floatingText != null && floatingText.activeSelf)
        {
            floatingText.SetActive(false);
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
        // Base interaction behavior (can be overridden by child scripts)
        Debug.Log($"{gameObject.name} interacted with. Override this method for specific behavior.");
    }
}
