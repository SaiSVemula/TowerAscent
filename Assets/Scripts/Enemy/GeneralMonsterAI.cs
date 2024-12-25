using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralMonsterAI : MonoBehaviour
{
    public float detectionRadius = 15f; // Radius for detecting the player
    public float battleTriggerRadius = 5f; // Radius to trigger a battle
    public float patrolSpeed = 2f; // Speed of patrolling
    public float walkInterval = 3f; // Time between patrols
    public float walkRadius = 5f; // Radius for random patrolling within a specific area
    public float rotationSpeed = 2f; // Speed of rotation during patrol
    public float patrolRange = 10f; // Maximum distance from the home position
    public Transform player; // Reference to the player's transform

    private bool isBattleTriggered = false; // Ensure the battle triggers only once
    private Animator animator; // Reference to the Animator component
    private Vector3 patrolTarget; // Target position for patrolling
    private Vector3 homePosition; // Initial position of the spider (patrol area center)
    private float walkTimer; // Timer to manage patrol intervals

    void Start()
    {
        // Locate the player by tag
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
        }

        // Assign the Animator component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on the spider.");
        }

        // Save the home position for patrol constraints
        homePosition = transform.position;

        // Set initial patrol target
        patrolTarget = homePosition;
    }

    void Update()
    {
        if (player == null || isBattleTriggered) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRadius)
        {
            Patrol(); // Resume walking in a random area within the patrol range
        }
        else if (distanceToPlayer <= detectionRadius && distanceToPlayer > battleTriggerRadius)
        {
            FacePlayer();
            StopMoving(); // Stop moving and play idle animation
        }
        else if (distanceToPlayer <= battleTriggerRadius)
        {
            TriggerBattle(); // Trigger attack animation and battle
        }
    }

    private void Patrol()
    {
        walkTimer += Time.deltaTime;

        if (walkTimer >= walkInterval || Vector3.Distance(transform.position, patrolTarget) < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
            randomDirection.y = 0f;
            patrolTarget = homePosition + randomDirection;

            if (Vector3.Distance(homePosition, patrolTarget) > patrolRange)
            {
                patrolTarget = homePosition + (patrolTarget - homePosition).normalized * patrolRange;
            }

            walkTimer = 0f;
        }

        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, patrolSpeed * Time.deltaTime);

        Vector3 direction = (patrolTarget - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Set isWalking to true
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }


    private void StopMoving()
    {
        // Set isWalking to false in the Animator
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Switch to idle animation
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }


    private void TriggerBattle()
    {
        if (isBattleTriggered) return;

        isBattleTriggered = true;
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
        }

        Debug.Log("Battle triggered with spider!");
    }

}