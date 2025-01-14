using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerPrefab;
    [SerializeField] private EnemyBattle enemyPrefab;
    [SerializeField] private CompanionBattle companionInstance;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private BattleUI battleUI;

    private PlayerBattle playerInstance;
    private EnemyBattle enemyInstance;
    private bool isPlayerTurn = true;

    private LevelLoader levelLoader;

    public Animator PlayerAnimator;
    public Animator GreenDragonAnimator;
    public Animator RedDragonAnimator;
    public Animator IceBossAnimator;

    public Animator comp1Animator;
    public Animator comp2Animator;
    public Animator comp3Animator;


    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();

        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }

        battleUI.OnStartBattle += HandleStartBattle;

        //PlayerAnimator = playerInstance.GetComponent.transform.Find("RPGH")<Animator>();
        comp1Animator = companionInstance.transform.Find("companion1").GetComponent<Animator>();
        comp2Animator = companionInstance.transform.Find("companion2").GetComponent<Animator>();
        comp3Animator = companionInstance.transform.Find("companion3").GetComponent<Animator>();
    }

    private void HandleStartBattle()
    {
        SpawnPlayer();
        SpawnCompanion();
        SpawnCorrectEnemy();

        // Initialize the battle after entities spawn
        StartCoroutine(InitializeBattle());
    }

    public IEnumerator InitializeBattle()
    {
        if (battleUI == null || enemyPrefab == null || playerPrefab == null || playerSpawnPoint == null || enemySpawnPoint == null)
        {
            Debug.LogError("Critical references missing in BattleManager!");
            yield break;
        }

        Debug.Log("Initializing battle...");
        playerInstance.gameObject.SetActive(true);
        enemyInstance.gameObject.SetActive(true);


        yield return StartCoroutine(battleUI.ShowCountdown());

        battleUI.EnableCardInteractions();
        battleUI.RenderCards(playerInstance.CardLoadout);
        battleUI.AddBattleLog($"Battle Start! Enemy: {enemyInstance.EnemyName} with {enemyInstance.CurrentHealth} HP");
        battleUI.UpdateTurnIndicator(isPlayerTurn);

        Debug.Log("Battle initialization complete.");
    }

    private void SpawnPlayer()
    {
        // Try to find an existing PlayerBattle object in the scene
        playerInstance = FindObjectOfType<PlayerBattle>();

        if (playerInstance == null)
        {
            // Instantiate the PlayerPrefab if not found
            playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            playerInstance.name = "Player";
            Debug.Log("Player instance created from prefab.");
        }
        else
        {
            // Use the found instance and reposition it
            playerInstance.gameObject.SetActive(true);
            playerInstance.transform.position = playerSpawnPoint.position;
            playerInstance.transform.rotation = Quaternion.identity;
            Debug.Log("Player instance found and repositioned.");
        }

        // Assign the player's animator to PlayerAnimator
        PlayerAnimator = playerInstance.GetComponentInChildren<Animator>();
        if (PlayerAnimator == null)
        {
            Debug.LogError("Animator component not found on player instance!");
        }
        else
        {
            Debug.Log("PlayerAnimator successfully assigned.");
        }
    }

    private void SpawnCompanion()
    {
        if (companionInstance == null)
        {
            Debug.LogError("Companion prefab is not assigned in the Inspector!");
            return;
        }

        CompanionType companionType = GameManager.Instance.GetCompanionType();
        Debug.Log($"Retrieved companion type: {companionType}");

        if (companionType == CompanionType.None)
        {
            Debug.Log("No companion selected. Skipping companion spawn.");
            companionInstance.gameObject.SetActive(false);
            return;
        }

        CompanionBattle companionBattle = companionInstance.GetComponent<CompanionBattle>();
        if (companionBattle == null)
        {
            Debug.LogError("CompanionPrefab does not have a CompanionBattle component!");
            return;
        }

        companionBattle.Initialize(companionType);
        SetupCompanionVisuals(companionType);

        companionInstance.transform.position = playerSpawnPoint.position + Vector3.right * 2;
        companionInstance.transform.rotation = Quaternion.identity;
        companionInstance.gameObject.SetActive(true);

        Debug.Log($"Spawned companion: {companionType} with {companionBattle.MaxHealth} HP.");
    }


    private void SetupCompanionVisuals(CompanionType companionType)
    {
        Transform companion1Avatar = companionInstance.transform.Find("companion1");
        Transform companion2Avatar = companionInstance.transform.Find("companion2");
        Transform companion3Avatar = companionInstance.transform.Find("companion3");

        if (companion1Avatar != null)
            companion1Avatar.gameObject.SetActive(companionType == CompanionType.Companion1);

        if (companion2Avatar != null)
            companion2Avatar.gameObject.SetActive(companionType == CompanionType.Companion2);

        if (companion3Avatar != null)
            companion3Avatar.gameObject.SetActive(companionType == CompanionType.Companion3);
    }

    private void SpawnCorrectEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemyPrefab is not assigned in the Inspector!");
            return;
        }

        // Ensure the prefab's GameObject is initially disabled
        enemyPrefab.gameObject.SetActive(false);

        // Determine the enemy type
        EnemyType enemyType = DetermineEnemyType();

        // Get the EnemyBattle component from the prefab
        EnemyBattle enemyBattle = enemyPrefab.GetComponent<EnemyBattle>();
        if (enemyBattle == null)
        {
            Debug.LogError("EnemyPrefab does not have an EnemyBattle component!");
            return;
        }

        // Initialize the enemy with the correct type and difficulty
        int difficulty = PlayerPrefs.GetInt("PlayersGameDifficulty", 0);
        enemyBattle.Initialize(difficulty, enemyType);

        // Optionally configure visuals if needed
        SetupEnemyVisuals(enemyType);

        // Set the position and enable the enemy
        enemyPrefab.transform.position = enemySpawnPoint.position;
        //enemyPrefab.transform.rotation = Quaternion.identity; // Reset rotation
        enemyPrefab.gameObject.SetActive(true);

        // Update the active enemy reference
        enemyInstance = enemyBattle;

        Debug.Log($"Spawned enemy: {enemyBattle.EnemyName} with {enemyBattle.MaxHealth} HP.");
    }

    private void SetupEnemyVisuals(EnemyType enemyType)
    {
        Transform greenAvatar = enemyPrefab.transform.Find("Green");
        Transform redAvatar = enemyPrefab.transform.Find("Red");
        Transform lichAvatar = enemyPrefab.transform.Find("FreeLichHP");

        if (greenAvatar == null || redAvatar == null || lichAvatar == null)
        {
            Debug.LogWarning("One or more enemy avatars are missing in the prefab.");
        }

        if (greenAvatar)
        {
            greenAvatar.gameObject.SetActive(enemyType == EnemyType.Enemy1);
        }
        if (redAvatar)
        {
            redAvatar.gameObject.SetActive(enemyType == EnemyType.Enemy2);
        }
        if (lichAvatar)
        {
            lichAvatar.gameObject.SetActive(enemyType == EnemyType.Enemy3);
        }
    }

    private EnemyType DetermineEnemyType()
    {
        switch (GameManager.Instance.PreviousScene)
        {
            case "Level 0":
                return EnemyType.Enemy1;
            case "Level 1":
                return EnemyType.Enemy2;
            case "Level 2":
                return EnemyType.Enemy3;
            default:
                Debug.LogError("Invalid PreviousScene value!");
                return EnemyType.Enemy1; // Default to Enemy1
        }
    }

    public void OnPlayerUseCard(int cardIndex)
    {
        if (!isPlayerTurn)
        {
            Debug.LogWarning("It's not the player's turn!");
            return;
        }

        if (cardIndex < 0 || cardIndex >= playerInstance.CardLoadout.Count)
        {
            Debug.LogError("Invalid card index!");
            return;
        }

        Card selectedCard = playerInstance.CardLoadout[cardIndex];
        if (selectedCard == null)
        {
            Debug.LogError("Selected card is null!");
            return;
        }

        // End the player's turn immediately to prevent further interactions
        isPlayerTurn = false;

        // Execute card logic
        string logMessage = selectedCard.Use(playerInstance, enemyInstance);
        battleUI.AddBattleLog(logMessage);

        // Disable all card buttons
        Transform cardPanel = battleUI.cardPanel.transform;
        foreach (Transform card in cardPanel)
        {
            Button cardButton = card.GetComponent<Button>();
            if (cardButton != null)
            {
                cardButton.interactable = false;
            }
        }

        // Proceed to animations and effects
        StartCoroutine(HandlePlayerCardAnimationAndTransition(selectedCard));
    }

    private IEnumerator HandlePlayerCardAnimationAndTransition(Card selectedCard)
    {
        // Trigger player attack animation
        PlayerAnimator.SetTrigger("Attack");
        AudioManager.instance.PlaySFX(4);

        // Short delay for the animation to start
        yield return new WaitForSeconds(0.2f);

        // Trigger enemy hit animations
        GreenDragonAnimator.SetTrigger("GetHit");
        RedDragonAnimator.SetTrigger("GetHit");
        IceBossAnimator.SetTrigger("GetHit");
        AudioManager.instance.PlaySFX(3);

        // Update effect timers for the player
        playerInstance.DecrementEffectTimers();

        // Check if the enemy is defeated
        if (enemyInstance.CurrentHealth <= 0)
        {
            Debug.Log("Enemy defeated!");

            // Play enemy death animations
            GreenDragonAnimator.SetTrigger("Die");
            RedDragonAnimator.SetTrigger("Die");
            IceBossAnimator.SetTrigger("Die");

            // End the battle
            yield return StartCoroutine(EndBattle(true));
            yield break;
        }

        // Wait for animations to complete before transitioning to the enemy's turn
        yield return new WaitForSeconds(1f);

        // Refresh the player�s cards after animations
        battleUI.RenderCards(playerInstance.CardLoadout);

        // Proceed to companion and enemy turns
        StartCoroutine(CompanionAndEnemyTurn(selectedCard));
    }

    private IEnumerator CompanionAndEnemyTurn(Card selectedCard)
    {
        // Companion attacks first if alive
        if (companionInstance != null && companionInstance.CurrentHealth > 0)
        {
            companionInstance.AttackEnemy(enemyInstance, battleUI);
            Debug.Log("Companion attacked the enemy.");

            // Trigger companion attack animations
            comp1Animator.SetTrigger("Attack");
            comp2Animator.SetTrigger("Attack");
            comp3Animator.SetTrigger("Attack");

            // Wait for the companion's attack animation
            yield return new WaitForSeconds(1f);
        }

        // Check if enemy is defeated
        if (enemyInstance.CurrentHealth <= 0)
        {
            Debug.Log("Enemy defeated by companion!");

            // Play enemy death animations
            GreenDragonAnimator.SetTrigger("Die");
            RedDragonAnimator.SetTrigger("Die");
            IceBossAnimator.SetTrigger("Die");

            yield return StartCoroutine(EndBattle(true));
            yield break;
        }

        // Enemy's turn
        yield return StartCoroutine(EnemyTurnWithDelay(selectedCard));

        // Reset cards for the player's turn
        //battleUI.RenderCards(playerInstance.CardLoadout);

        // Player's turn starts again
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        //battleUI.EnableCardInteractions();
    }
    private IEnumerator EnemyTurnWithDelay(Card selectedCard)
    {
        yield return new WaitForSeconds(1f);

        if (enemyInstance == null)
        {
            Debug.LogError("enemyInstance is null in EnemyTurnWithDelay!");
            yield break;
        }

        if (playerInstance == null && companionInstance == null)
        {
            Debug.LogError("Both playerInstance and companionInstance are null in EnemyTurnWithDelay!");
            yield break;
        }

        // Enemy uses a card or action
        if (enemyInstance.CardLoadout == null || enemyInstance.CardLoadout.Count == 0)
        {
            Debug.LogError($"{enemyInstance.EnemyName} has no cards in its loadout.");
            yield break;
        }

        // Select a random card from the enemy's card loadout
        int cardIndex = Random.Range(0, enemyInstance.CardLoadout.Count);
        Card selectedEnemyCard = enemyInstance.CardLoadout[cardIndex];

        if (selectedEnemyCard == null)
        {
            Debug.LogError("Selected card for the enemy is null.");
            yield break;
        }

        // Enemy attacks the player
        if (playerInstance != null && playerInstance.CurrentHealth > 0)
        {
            string logMessagePlayer = selectedEnemyCard.Use(enemyInstance, playerInstance);
            battleUI.AddBattleLog($"{enemyInstance.EnemyName} used {selectedEnemyCard.Name} on {playerInstance.name}. {logMessagePlayer}");
            Debug.Log($"{enemyInstance.EnemyName} attacked Player: {logMessagePlayer}");

            GreenDragonAnimator.SetTrigger("Attack");
            RedDragonAnimator.SetTrigger("Attack");
            IceBossAnimator.SetTrigger("Attack");

            PlayerAnimator.SetTrigger("GetHit");
            AudioManager.instance.PlaySFX(3);
        }

        // Enemy attacks the companion (if present and alive)
        if (companionInstance != null && companionInstance.CurrentHealth > 0)
        {
            string logMessageCompanion = selectedEnemyCard.Use(enemyInstance, companionInstance);
            battleUI.AddBattleLog($"{enemyInstance.EnemyName} used {selectedEnemyCard.Name} on {companionInstance.CompanionName}. {logMessageCompanion}");
            Debug.Log($"{enemyInstance.EnemyName} attacked Companion: {logMessageCompanion}");

            comp1Animator.SetTrigger("GetHit");
            comp2Animator.SetTrigger("GetHit");
            comp3Animator.SetTrigger("GetHit");
        }

        // Update effect timers for the enemy after its turn
        enemyInstance.DecrementEffectTimers();
        Debug.Log("Enemy effect timers decremented.");

        // Check if the player or companion is defeated
        if (playerInstance.CurrentHealth <= 0)
        {
            Debug.Log("Player has been defeated!");

            PlayerAnimator.SetTrigger("Die");
            AudioManager.instance.PlaySFX(2);

            GreenDragonAnimator.SetTrigger("Win");
            RedDragonAnimator.SetTrigger("Win");
            IceBossAnimator.SetTrigger("Win");

            yield return StartCoroutine(EndBattle(false));
            yield break;
        }

        if (companionInstance != null && companionInstance.CurrentHealth <= 0)
        {
            comp1Animator.SetTrigger("Die");
            comp2Animator.SetTrigger("Die");
            comp3Animator.SetTrigger("Die");

            AudioManager.instance.PlaySFX(2);

            Debug.Log("Companion has been defeated!");
            battleUI.AddBattleLog($"{companionInstance.CompanionName} has been defeated!");
        }
    }

    public IEnumerator EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");

        // Save battle results
        SaveBattleResults(playerWon);

        // Display the battle result
        yield return StartCoroutine(battleUI.ShowBattleResult(playerWon));

        yield return new WaitForSeconds(3f);

        // Disable player interactions after the battle
        isPlayerTurn = false;

        // Transition to the next scene
        Debug.Log("Transitioning to next scene...");
        string nextScene = "";

        if (playerWon)
        {
            nextScene = GameManager.Instance.NextScene;

            if (nextScene == "Level1")
            {
                // Get objectives for Level 1
                List<string> level1Objectives = GameManager.Instance.GetObjectivesForScene("Level1");

                // Load Level 1 with objectives
                GameManager.Instance.SaveObjectives(level1Objectives);
                
            }
            else
            {
                levelLoader.LoadScene("BattleScene", "LevelTransitionCutScene");
            }
        }
        else
        {
            // Return to the previous scene if the player loses
            nextScene = GameManager.Instance.PreviousScene;
            levelLoader.LoadScene("BattleScene", nextScene);
        }
    }


    private void SaveBattleResults(bool playerWon)
    {
        // Save the player's coins
        int difficulty = PlayerPrefs.GetInt("PlayersGameDifficulty", 0);
        int rewardAmount = 0;
        switch (difficulty) {
            case 0:
                rewardAmount = 10;
                break;
            case 1: 
                rewardAmount = 15;
                break;
            case 2:
                rewardAmount = 25;
                break;
        }

        // Save the enemy's defeat
        if (playerWon)
        {
            GameManager.Instance.RewardGold(rewardAmount);
            string defeatedEnemy = enemyInstance.EnemyName;
            GameManager.Instance.CompleteObjective($"Defeated {defeatedEnemy}");
        }

        // Save any additional data, like if player unlocked a new card
        GameManager.Instance.UpdatePlayerCards(playerInstance.CardLoadout.Select(c => c.Name).ToArray());

        GameManager.Instance.CurrentCardLoadout = null;

        // Log save operation
        Debug.Log("Battle results saved to GameManager.");
    }
}
