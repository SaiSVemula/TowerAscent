using System.Collections;
using UnityEngine;

public class GeneralMonsterAI : MonoBehaviour
{
    public float detectionRadius = 15f; // Radius for detecting the player
    public float battleTriggerRadius = 5f; // Radius to trigger a battle
    public float patrolSpeed = 2f; // Speed of walking
    public float rotationSpeed = 5f; // Speed of rotation toward the player
    public Transform player; // Reference to the player's transform
    public PlayerMovement playerMovementScript; // Reference to the player's movement script

    private bool isBattleTriggered = false; // Ensure the battle triggers only once
    private Animator animator; // Reference to the Animator component

    void Start()
    {
        // Locate the player by tag
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
        }

        // Locate the player's movement script
        if (player != null)
        {
            playerMovementScript = player.GetComponent<PlayerMovement>();
            if (playerMovementScript == null)
            {
                Debug.LogError("PlayerMovement script not found on the Player object.");
            }
        }

        // Assign the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the spider.");
        }

        // Ensure the spider starts in the Idle animation
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }

    void Update()
    {
        if (player == null || isBattleTriggered) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRadius)
        {
            StopMoving(); // Stay idle
        }
        else if (distanceToPlayer <= detectionRadius && distanceToPlayer > battleTriggerRadius)
        {
            FacePlayer();
            StopMoving(); // Stop moving and play idle animation
        }
        else if (distanceToPlayer <= battleTriggerRadius)
        {
            StartCoroutine(WalkToPlayerAndAttack()); // Start walking and attacking sequence
        }
    }

    private void StopMoving()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    private IEnumerator WalkToPlayerAndAttack()
    {
        if (isBattleTriggered) yield break;

        isBattleTriggered = true;

        // Disable player's movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }

        while (Vector3.Distance(transform.position, player.position) > 2.0f)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
        }

        yield return new WaitForSeconds(1.5f);

        Debug.Log("Battle triggered with spider!");

        // Enable player's movement after battle (if needed)
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }
}
