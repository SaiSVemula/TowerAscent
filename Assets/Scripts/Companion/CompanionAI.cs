using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform CurrentPlayer; // Reference to the player
    public GameObject floatingText; // Floating text to show when nearby

    private bool isBefriended = false;
    private Rigidbody CompanionRigidbody;
    private Animator animator;

    public float DefaultFollowDistance = 3f; // Distance to maintain from the player
    public float FollowSpeed = 5f; // Walking speed
    public float CatchUpSpeed = 8f; // Catch-up speed
    public float CatchUpThreshold = 4.5f; // Distance threshold for catch-up speed
    public float RunningSpeedMultiplier = 2.5f; // Speed multiplier when player is running
    public float StoppingDistance = 1.8f; // Adjusted to prevent jittering
    public float RotationSpeed = 5f; // Smoother rotation

    void Start()
    {
        CompanionRigidbody = GetComponent<Rigidbody>();
        CompanionRigidbody.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from the companion.");
        }
    }

    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false); // Hide text when befriended
            FollowPlayer();
        }
        else
        {
            animator.SetFloat("Speed", 0f); // Reset to idle if not befriended
        }
    }

    private void FollowPlayer()
    {
        // Target position
        Vector3 targetPosition = CurrentPlayer.position - CurrentPlayer.forward * DefaultFollowDistance;

        // Align Y position to the ground using raycast
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.CompareTag("Floor"))
            {
                targetPosition.y = hitInfo.point.y + 0.1f; // Adjust height to ground level
            }
        }

        // Calculate distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Stop moving when within the stopping distance
        if (distanceToTarget <= StoppingDistance)
        {
            CompanionRigidbody.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f); // Play idle animation
            return;
        }

        // Determine speed based on distance
        float speed = FollowSpeed;
        if (distanceToTarget > CatchUpThreshold)
        {
            speed = CatchUpSpeed;
        }

        // Apply running multiplier if the player is running
        if (Input.GetKey(KeyCode.LeftShift) && IsPlayerMoving())
        {
            speed *= RunningSpeedMultiplier;
        }

        // Movement direction
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Smoothly rotate to face movement direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);
        }

        // Move towards the target position
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        CompanionRigidbody.MovePosition(newPosition);

        // Update the animator's Speed parameter for transitions
        float animationSpeed = Mathf.Clamp(speed / CatchUpSpeed, 0.5f, 1f); // Normalize speed
        animator.SetFloat("Speed", animationSpeed);
    }

    public void Befriend()
    {
        floatingText.SetActive(false);
        isBefriended = true;
    }

    private bool IsPlayerMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
