using UnityEngine;

/*This Code Handles The CheckPoints Behaviour
From When It is Clicked And White Turns To Green
And How It Saves The Players Location To Be Persisently Saved
Later*/
public class OnCheckpointHit : MonoBehaviour
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
        if (collidedItem.CompareTag("Player")) // if it was a player not a companion or monster that hit the checkpoint then save
        {
            GameManager.Instance.UpdateCurrentScene(); // updates the current scene so we know what scene to resume back onto

            // Update the player's location into gamemanager
            Transform CollidedPlayer = collidedItem.transform;
            Vector3 coordToSave = CollidedPlayer.position;
            GameManager.Instance.UpdatePlayerLocation(coordToSave);
            GameManager.Instance.SavePlayerState(); // updates players stats to gamemanager

            SaveManager.SaveGameState(); // save gamemanagers updated fields to persistent storage

            SetCheckpointState(true); // Sets Checkpoint to Green (Set)
        }
    }

    private void SetCheckpointState(bool CheckpointHit)// Method to Set The Checkpoint Objects To Green Or White
    {
        // Hide when Checkpoint Hit
        if (whiteBox != null) { whiteBox.SetActive(!CheckpointHit);  }
        if (whiteParticle != null) { whiteParticle.SetActive(!CheckpointHit); }

        // Show when Checkpoint Hit
        if (greenBox != null)  { greenBox.SetActive(CheckpointHit);}
        if (greenParticle != null) { greenParticle.SetActive(CheckpointHit); }
    }
}
