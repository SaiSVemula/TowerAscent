using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    public Transform CurrentPlayer; // Reference to the player
    public GameObject floatingText; // Floating text to show when nearby

    private bool isBefriended = false;
    private Rigidbody CompanionRigidbody;
    private Animator animator;

    public float DefaultFollowDistance = 3f; // Distance to maintain from the player
    public float FollowSpeed = 5f; // Normal walking speed
    public float CatchUpSpeed = 8f; // Increased speed when the player is far away
    public float FarDistanceThreshold = 6f; // Distance threshold for faster speed
    public float StoppingDistance = 1.8f; // Stop distance to prevent jittering
    public float RotationSpeed = 5f; // Smooth rotation speed

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
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, CurrentPlayer.position);

        // Stop moving when within the stopping distance
        if (distanceToPlayer <= StoppingDistance)
        {
            CompanionRigidbody.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f); // Play idle animation
            return;
        }

        // Determine speed based on distance
        float speed = (distanceToPlayer > FarDistanceThreshold) ? CatchUpSpeed : FollowSpeed;

        // Movement direction
        Vector3 direction = (CurrentPlayer.position - transform.position).normalized;

        // Smoothly rotate to face the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

        // Move towards the player
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        CompanionRigidbody.MovePosition(newPosition);

        // Update the animator's Speed parameter for transitions
        float normalizedSpeed = Mathf.Clamp(speed / CatchUpSpeed, 0f, 1f);
        animator.SetFloat("Speed", normalizedSpeed); // Use RunForward animation
    }

    public void Befriend()
    {
        floatingText.SetActive(false);
        isBefriended = true;
    }
}
