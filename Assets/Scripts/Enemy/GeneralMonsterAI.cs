using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GeneralMonsterAI : BattleEntity
{
    // Spider Behavior
    public float detectionRadius = 15f; // Radius for detecting the player
    public float battleTriggerRadius = 5f; // Radius to trigger a battle
    public float patrolSpeed = 2f; // Speed of walking
    public float rotationSpeed = 5f; // Speed of rotation toward the player
    public Vector3 originalSpawnPosition;
    private Transform player; // Reference to the player's transform
    private PlayerMovement playerMovementScript; // Reference to the player's movement script
    private bool isBattleTriggered = false; // Ensure the battle triggers only once

    // Health Slider
    [SerializeField] public GameObject healthBarPrefab; // Reference to the health bar prefab
    [SerializeField] public Slider EnemyHealthBar; // Reference to the health bar slider
    
    // Spider-Specific Details
    public string spiderID; // Unique ID for each spider
    public ParticleSystem deathEffect; // Effect when spider is defeated

    [SerializeField] public MiniBattleManager miniBattleManager;

    void Start()
    {
        // Locate the player and related scripts
        player = GameObject.FindWithTag("Player")?.transform;
        originalSpawnPosition = transform.position;
        if (player == null)
        {
            Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
            return;
        }

        healthBar = EnemyHealthBar;

        maxHealth = 50;
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; 
            UpdateHealthBar(); // Calling parent method
        }


        playerMovementScript = player.GetComponent<PlayerMovement>();
        if (playerMovementScript == null)
        {
            Debug.LogError("PlayerMovement script not found on the Player object.");
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

        // Check if the spider has been defeated already
        if (GameManager.Instance.IsSpiderDefeated(spiderID))
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (player == null || isBattleTriggered)
        {
            return;
        }

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
            StartCoroutine(WalkToPlayerAndTriggerMiniBattle()); // Start walking and triggering sequence
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

    private IEnumerator WalkToPlayerAndTriggerMiniBattle()
    {
        if (isBattleTriggered)
        {
            yield break;
        }

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

        // Trigger the mini-battle
        TriggerMiniBattle();
    }
    public void AttackPlayer(BattleEntity playerEntity)
    {
        if (playerEntity == null)
        {
            Debug.LogError("Player entity is null.");
            return;
        }

        int damage = 10;                                              ///////////////////////////////////// UPDATE THIS TO HAVE A CARD OR A RANDOM NUMBER
        playerEntity.TakeDamage(damage);

        Debug.Log($"Spider attacked {playerEntity.name} and dealt {damage} damage.");
    }

    private void TriggerMiniBattle()
    {
        Debug.Log("Mini-battle triggered!");

        transform.position = originalSpawnPosition;

        // Disable player's movement
        if (playerMovementScript != null)
        {
            playerMovementScript.LockMovement(true); 
        }

        miniBattleManager.SendMessage("StartMiniBattle", this);
    }

    public void SetUpMiniBattle()
    {
        int difficulty = PlayerPrefs.GetInt("GameDifficulty", 0);
        if(difficulty == 0)
        {
            maxHealth = 25;
        }
        else if(difficulty == 1)
        {
            maxHealth = 50;
        }
        else if(difficulty == 2)
        {
            maxHealth = 75;
        }

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // Set the maximum health value
            healthBar.value = currentHealth; // Set the current health value
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            DefeatSpider();
        }
    }


    public void DefeatSpider()
    {
        // Play particle effect
        Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Log defeat in GameManager
        Debug.Log($"Spider with ID {spiderID} defeated. Marking it as defeated.");
        GameManager.Instance.MarkSpiderDefeated(spiderID);

        // Remove spider and health bar from the scene
        if (healthBar != null)
        {
            healthBarPrefab.SetActive(false);
            Destroy(healthBar.gameObject);
        }

        Destroy(gameObject);
    }

}
