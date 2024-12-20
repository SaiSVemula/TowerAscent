using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform CurrentPlayer; // Reference to the player
    public GameObject floatingText; // Floating text to show when nearby
    private bool isBefriended = false;
    private Rigidbody CompanionRigidbody;

    public float DefaultFollowDistance = 1.8f; // Distance to maintain from the player
    public float FollowSpeed = 16f; // Walking speed
    public float CatchUpSpeed = 23f; // Catch-up speed for long distances
    public float CatchUpThreshold = 3f; // Distance threshold for catch-up speed
    public float RunningSpeedMultiplier = 2.5f; // Speed multiplier when the player is running
    public float StoppingDistance = 0.5f; // Distance threshold to stop jittering

    private Vector3 lastPosition; // Track the companion's last position for smooth movement

    void Start()
    {
        CompanionRigidbody = GetComponent<Rigidbody>();
        CompanionRigidbody.freezeRotation = true;
    }

    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false);
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Calculate the target position behind the player
        Vector3 targetPosition = CurrentPlayer.position - CurrentPlayer.forward * DefaultFollowDistance;
        targetPosition.y = transform.position.y; // Keep companion at the same height

        // Calculate the distance to the target position
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // If within stopping distance, don't move and reset velocity
        if (distanceToTarget <= StoppingDistance)
        {
            CompanionRigidbody.velocity = Vector3.zero;
            return; // Stop further calculations
        }

        // Determine movement speed
        float speed = FollowSpeed; // Default walking speed
        if (distanceToTarget > CatchUpThreshold)
        {
            speed = CatchUpSpeed; // Use catch-up speed if distance is too great
        }

        // If the player is running, multiply the speed
        if (Input.GetKey(KeyCode.LeftShift) && IsPlayerMoving())
        {
            speed *= RunningSpeedMultiplier;
        }

        // Move towards the target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Apply smooth movement to prevent jittering
        if (Vector3.Distance(lastPosition, newPosition) > 0.01f) // Threshold to avoid tiny adjustments
        {
            CompanionRigidbody.MovePosition(newPosition);
            lastPosition = transform.position; // Update the last position
        }
    }

    public void Befriend()
    {
        floatingText.SetActive(false);
        isBefriended = true;
    }

    private bool IsPlayerMoving()
    {
        // Check if the player is pressing any movement keys
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) ||
               Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
    }
}
