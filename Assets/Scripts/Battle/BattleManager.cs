using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    }

    private void SpawnCompanion()
    {
        if (companionInstance == null)
        {
            Debug.LogError("Companion prefab is not assigned in the Inspector!");
            return;
        }

        // Ensure the prefab's GameObject is initially disabled
        companionInstance.gameObject.SetActive(false);

        // Determine the companion type
        //int companionTypeInt = PlayerPrefs.GetInt("PlayerCompanionType", 0); // Default to Companion1
        //CompanionType companionType = (CompanionType)companionTypeInt;

        // get companion added from game manager
        CompanionType companionType = GameManager.Instance.GetCompanionType();

        // Get the CompanionBattle component from the prefab
        CompanionBattle companionBattle = companionInstance.GetComponent<CompanionBattle>();
        if (companionBattle == null)
        {
            Debug.LogError("CompanionPrefab does not have a CompanionBattle component!");
            return;
        }

        // Initialize the companion with the correct type
        companionBattle.Initialize(companionType);

        // Optionally configure visuals if needed
        SetupCompanionVisuals(companionType);

        // Set the position and enable the companion
        companionInstance.transform.position = playerSpawnPoint.position + Vector3.right * 2; // Adjust spawn offset
        companionInstance.transform.rotation = Quaternion.identity; // Reset rotation
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

        string logMessage = selectedCard.Use(playerInstance, enemyInstance);
        battleUI.AddBattleLog(logMessage);

        PlayerAnimator.SetTrigger("Attack");

        GreenDragonAnimator.SetTrigger("GetHit");        
        RedDragonAnimator.SetTrigger("GetHit");        
        IceBossAnimator.SetTrigger("GetHit");

        // Update effect timers after card usage
        playerInstance.DecrementEffectTimers();

        if (enemyInstance.CurrentHealth <= 0)
        {
            Debug.Log("Enemy defeated!");

            // Death animations
            GreenDragonAnimator.SetTrigger("Die");        
            RedDragonAnimator.SetTrigger("Die");        
            IceBossAnimator.SetTrigger("Die");

            StartCoroutine(EndBattle(true));
            return;
        }

        // Player's turn ends, companion and enemy turns occur
        StartCoroutine(CompanionAndEnemyTurn());
    }

    private IEnumerator CompanionAndEnemyTurn()
    {
        if (companionInstance != null && companionInstance.CurrentHealth > 0)
        {
            // Companion attacks first
            companionInstance.AttackEnemy(enemyInstance, battleUI);
            Debug.Log("Companion attacked the enemy.");
            yield return new WaitForSeconds(1f);

            // attack animations
            comp1Animator.SetTrigger("Attack");
            comp2Animator.SetTrigger("Attack");
            comp3Animator.SetTrigger("Attack");
        }

        // Check if enemy is defeated
        if (enemyInstance.CurrentHealth <= 0)
        {
            Debug.Log("Enemy defeated by companion!");
            StartCoroutine(EndBattle(true));
            yield break;
        }

        // Enemy's turn
        isPlayerTurn = false;
        yield return StartCoroutine(EnemyTurnWithDelay(null));

        // Player's turn starts again
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.RenderCards(playerInstance.CardLoadout);
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
        }

        // Enemy attacks the companion (if present and alive)
        if (companionInstance != null && companionInstance.CurrentHealth > 0)
        {
            string logMessageCompanion = selectedEnemyCard.Use(enemyInstance, companionInstance);
            battleUI.AddBattleLog($"{enemyInstance.EnemyName} used {selectedEnemyCard.Name} on {companionInstance.CompanionName}. {logMessageCompanion}");
            Debug.Log($"{enemyInstance.EnemyName} attacked Companion: {logMessageCompanion}");

            
            // gethit animations
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

            PlayerAnimator.SetTrigger("Die"); // Death animation

            // Win animations
            GreenDragonAnimator.SetTrigger("Win");        
            RedDragonAnimator.SetTrigger("Win");        
            IceBossAnimator.SetTrigger("Win");

            yield return StartCoroutine(EndBattle(false));
            yield break;
        }

        if (companionInstance != null && companionInstance.CurrentHealth <= 0)
        {
            // die animations
            comp1Animator.SetTrigger("Die");
            comp2Animator.SetTrigger("Die");
            comp3Animator.SetTrigger("Die");

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

        // Disable player interactions after the battle
        isPlayerTurn = false;

        // Transition to the next scene
        Debug.Log("Transitioning to next scene...");
        string nextScene = "";
        if (playerWon)
        {
            nextScene = GameManager.Instance.NextScene;
        }
        else
        {
            // lost so back to scene.
            nextScene = GameManager.Instance.PreviousScene;
        }
        levelLoader.LoadScene("BattleScene", nextScene);
        //levelLoader.LoadScene("BattleScene", "LevelTransitionCutScene");
    }

    private void SaveBattleResults(bool playerWon)
    {
        // Update the player's statistics
        //if (playerWon)
        //{
        //    GameManager.Instance.AddMiniBattleWin(); 
        //}
        //else
        //{
        //    GameManager.Instance.AddMiniBattleLoss();
        //}

        // Save the player's coins
        GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.CurrentCoins1 + 10);

        // Save the enemy's defeat
        if (playerWon)
        {
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
