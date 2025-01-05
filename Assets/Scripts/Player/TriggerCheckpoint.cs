using UnityEngine;

/*This Code Handles The CheckPoints Behaviour
From When It is Clicked And White Turns To Green
And How It Saves The Players Location To Be Persisently Saved
Later*/
public class TriggerCheckpoint : MonoBehaviour
{
    public GameObject whiteBox; // White box (checkpoint unset)
    public GameObject whiteParticle; // White particle system (checkpoint unset)
    public GameObject greenBox; // Green box (checkpoint set)
    public GameObject greenParticle; // Green particle system (checkpoint set)

    private void Start()
    {
        // Makes Sure We Start Off With White (Unset Checkpoint)
        // This Is Turned To Green Automatically On resume as player hits the checkpoint on load
        if (greenBox != null) {greenBox.SetActive(false);}
        if (greenParticle != null){greenParticle.SetActive(false);}
    }

    private void OnTriggerEnter(Collider collidedItem) // This Checks If A player collided and saves player location and stats
    {
        if (collidedItem.CompareTag("Player"))
        {
            // Update the current scene in GameManager
            GameManager.Instance.UpdateCurrentScene();

            // Update the player's location in GameManager
            Transform playerTransform = collidedItem.transform;
            Vector3 playerCoord = playerTransform.position;
            GameManager.Instance.UpdatePlayerLocation(playerCoord);
            GameManager.Instance.SavePlayerState();

            // Log the player's coordinates when the checkpoint is activated
            Debug.Log($"Checkpoint activated! Player coordinates: {playerCoord}");

            // Save the updated state
            SaveManager.SaveGameState();

            // Log the state save
            Debug.Log("Scene and player location updated and saved.");

            // Hide the white objects and show the green objects
            SetCheckpointState(true);
        }
    }

    // Method to hide/show the checkpoint objects
    private void SetCheckpointState(bool isActivated)
    {
        if (whiteBox != null)
        {
            whiteBox.SetActive(!isActivated);  // Hide when activated
        }

        if (whiteParticle != null)
        {
            whiteParticle.SetActive(!isActivated);  // Hide when activated
        }

        if (greenBox != null) 
        { 
            greenBox.SetActive(isActivated); // Show when activated
        }
        
        if (greenParticle != null)
        {
            greenParticle.SetActive(isActivated);  // Show when activated
        }
    }
}
