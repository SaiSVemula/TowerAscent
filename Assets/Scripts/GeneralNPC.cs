using UnityEngine;
using UnityEngine.UI;

public class GeneralNPC : MonoBehaviour
{
    [Header("Floating Text and Dialogue")]
    public GameObject floatingText; // Text to display when the player is near
    public GameObject dialogueUI; // Reference to the dialogue UI
    public Text dialogueText; // Reference to the Text component in the dialogue UI

    [Header("Detection Settings")]
    public float detectionRadius = 10f; // Radius for detecting the player
    private Transform player; // Reference to the player's transform

    [Header("Subtitle Settings")]
    public string[] dialogueLines; // Dialogue lines for the NPC
    public float subtitleDisplayDuration = 3f; // Duration to display each subtitle
    private bool isInteracting = false; // Is the player currently interacting

    protected virtual void Start()
    {
        // Disable floating text and dialogue UI at the start
        if (floatingText != null)
        {
            floatingText.SetActive(false);
        }

        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
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
        if (player == null || isInteracting) return;

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
            StartInteraction();
        }
    }

    protected virtual void OnPlayerOutOfRange()
    {
        // Disable floating text and end interaction when out of range
        if (floatingText != null && floatingText.activeSelf)
        {
            floatingText.SetActive(false);
        }
        if (isInteracting)
        {
            EndInteraction();
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

    public virtual void StartInteraction()
    {
        isInteracting = true;

        // Disable floating text
        if (floatingText != null)
        {
            floatingText.SetActive(false);
        }

        // Enable dialogue UI
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(true);
        }

        // Display a random dialogue line
        DisplayRandomDialogueLine();
    }

    public virtual void EndInteraction()
    {
        isInteracting = false;

        // Disable dialogue UI
        if (dialogueUI != null)
        {
            dialogueUI.SetActive(false);
        }
    }

    private void DisplayRandomDialogueLine()
    {
        if (dialogueLines.Length > 0)
        {
            // Pick a random dialogue line
            int randomIndex = Random.Range(0, dialogueLines.Length);
            string randomDialogue = dialogueLines[randomIndex];

            Debug.Log($"NPC Dialogue: {randomDialogue}");

            // Display dialogue in the UI
            if (dialogueText != null)
            {
                dialogueText.text = randomDialogue;
            }

            // Schedule the end of interaction after displaying the dialogue
            Invoke(nameof(EndInteraction), subtitleDisplayDuration);
        }
        else
        {
            Debug.LogWarning("No dialogue lines assigned to this NPC.");
            EndInteraction();
        }
    }
}
