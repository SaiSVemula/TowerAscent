using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform CurrentPlayer; // Reference to the player
    public GameObject floatingText; // Floating text to show when nearby
    private bool isBefriended = false;
    private Rigidbody CompanionRigidbody;
    private Animator animator; // Reference to the animator

    public float DefaultFollowDistance = 3f; // Distance to maintain from the player
    public float FollowSpeed = 5f; // Walking speed (lowered for testing)
    public float CatchUpSpeed = 8f; // Catch-up speed for long distances
    public float CatchUpThreshold = 4.5f; // Distance threshold for catch-up speed
    public float RunningSpeedMultiplier = 2.5f; // Speed multiplier when the player is running
    public float StoppingDistance = 1.5f; // Distance threshold to stop jittering

    private Vector3 lastPosition; // Track the companion's last position for smooth movement

    void Start()
    {
        CompanionRigidbody = GetComponent<Rigidbody>();
        CompanionRigidbody.freezeRotation = true;
        animator = GetComponentInChildren<Animator>(); // Assign the Animator component
    }

    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false);
            FollowPlayer();
        }
        else
        {
            animator.SetFloat("Speed", 0f); // Stop animation if not befriended
        }
    }

    private void FollowPlayer()
    {
        // Calculate the target position behind the player
        Vector3 targetPosition = CurrentPlayer.position - CurrentPlayer.forward * DefaultFollowDistance;

        // Check for the floor height using Raycast
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.CompareTag("Floor")) // Ensure we hit the floor
            {
                targetPosition.y = hitInfo.point.y + 0.1f; // Align the Y position to the floor's height
            }
        }

        // Calculate the distance to the target position
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // If within stopping distance, don't move and reset velocity
        if (distanceToTarget <= StoppingDistance)
        {
            CompanionRigidbody.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f); // Play idle animation
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

        // Update the companion's rotation to face the movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }

        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Apply smooth movement to prevent jittering
        if (Vector3.Distance(transform.position, newPosition) > 0.01f) // Threshold to avoid tiny adjustments
        {
            CompanionRigidbody.MovePosition(newPosition);
        }

        // Update the animator's Speed parameter
        animator.SetFloat("Speed", speed > FollowSpeed ? 1f : 0.5f); // Run or walk animation
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
