//using System.Collections;
//using UnityEngine;

//public class BattleManager : MonoBehaviour
//{
//    [SerializeField] private PlayerBattle playerBattle;
//    private EnemyBattle activeEnemy; // The actual enemy used in battle
//    [SerializeField] private Transform enemySpawnPoint; // Assign the position where the enemy will spawn
//    [SerializeField] private EnemyBattle enemyBattle; 
//    //[SerializeField] private Enemy1 enemy1Prefab;
//    //[SerializeField] private Enemy2 enemy2Prefab;
//    //[SerializeField] private Enemy3 enemy3Prefab;
//    [SerializeField] private BattleUI battleUI;

//    private bool isPlayerTurn = true;

//    private LevelLoader levelLoader;

//    private void Start()
//    {
//        GameManager.Instance.PreviousScene = "ExplorationScene";
//        GameManager.Instance.NextScene = "Level 1";
//        levelLoader = FindObjectOfType<LevelLoader>();

//        if (levelLoader == null)
//        {
//            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
//        }
//        StartCoroutine(InitializeBattle());
//    }

//    public IEnumerator InitializeBattle()
//    {
//        if (battleUI == null || playerBattle == null)
//        {
//            Debug.LogError("Critical references missing!");
//            yield break;
//        }

//        Debug.Log("Waiting for instructions to start battle...");

//        // Wait for instruction canvas to be completed
//        yield return new WaitUntil(() => battleUI.IsBattleReady);

//        Debug.Log("Instruction canvas completed, starting battle initialization...");

//        // Spawn the correct enemy
//        SpawnCorrectEnemy();

//        if (activeEnemy == null)
//        {
//            Debug.LogError("Enemy spawning failed!");
//            yield break;
//        }

//        yield return StartCoroutine(battleUI.ShowCountdown());

//        battleUI.RenderCards(playerBattle.CardLoadout);
//        battleUI.EnableCardInteractions();
//        battleUI.AddBattleLog($"Battle Start! Enemy: {activeEnemy.EnemyName} with {activeEnemy.CurrentHealth} HP");
//        battleUI.UpdateTurnIndicator(true);

//        Debug.Log("Battle initialization complete");
//    }


//    private void SpawnCorrectEnemy()
//    {
//        // Assume these prefabs or scene GameObjects are assigned in the inspector
//        GameObject spawnedEnemy = null;

//        // Disable all existing enemies initially
//        if (enemy1Prefab != null) enemy1Prefab.gameObject.SetActive(false);
//        if (enemy2Prefab != null) enemy2Prefab.gameObject.SetActive(false);
//        if (enemy3Prefab != null) enemy3Prefab.gameObject.SetActive(false);

//        // Determine which enemy to enable based on the scene
//        switch (GameManager.Instance.PreviousScene)
//        {
//            case "ExplorationScene":
//                spawnedEnemy = enemy1Prefab.gameObject; // Use Enemy1 prefab
//                break;
//            case "Level 1":
//                spawnedEnemy = enemy2Prefab.gameObject; // Use Enemy2 prefab
//                break;
//            case "Level 2":
//                spawnedEnemy = enemy3Prefab.gameObject; // Use Enemy3 prefab
//                break;
//            default:
//                Debug.LogError("Invalid PreviousScene value!");
//                return;
//        }

//        // Activate and initialize the correct enemy
//        if (spawnedEnemy != null)
//        {
//            spawnedEnemy.SetActive(true); // Activate the correct enemy
//            activeEnemy = spawnedEnemy.GetComponent<EnemyBattle>();

//            if (activeEnemy != null)
//            {
//                activeEnemy.Initialize(GameManager.Instance.GameDifficulty);

//                // Notify the UI about the active enemy
//                if (battleUI != null)
//                {
//                    battleUI.SetActiveEnemy(activeEnemy);
//                }
//                else
//                {
//                    Debug.LogError("BattleUI reference is missing!");
//                }

//                Debug.Log($"Enemy {activeEnemy.EnemyName} activated for battle.");
//            }
//            else
//            {
//                Debug.LogError("Spawned enemy does not have an EnemyBattle component!");
//            }
//        }
//        else
//        {
//            Debug.LogError("Failed to determine which enemy to spawn.");
//        }
//    }

//    public void OnPlayerUseCard(int cardIndex)
//    {
//        if (!isPlayerTurn)
//        {
//            return;
//        }

//        if (cardIndex < 0 || cardIndex >= playerBattle.CardLoadout.Count)
//        {
//            Debug.LogError("Invalid card index!");
//            return;
//        }

//        Card selectedCard = playerBattle.CardLoadout[cardIndex];
//        if (selectedCard == null)
//        {
//            Debug.LogError("Selected card is null!");
//            return;
//        }

//        string logMessage = selectedCard.Use(playerBattle, activeEnemy);
//        battleUI.AddBattleLog(logMessage);

//        if (activeEnemy.CurrentHealth <= 0)
//        {
//            StartCoroutine(EndBattle(true));
//            return;
//        }

//        battleUI.UpdateEffectTimers();
//        isPlayerTurn = false;
//        battleUI.UpdateTurnIndicator(isPlayerTurn);
//        StartCoroutine(EnemyTurnWithDelay());
//    }

//    private IEnumerator EnemyTurnWithDelay()
//    {
//        yield return new WaitForSeconds(1f); // Add a delay before enemy action

//        // Let the enemy attack the player
//        if (activeEnemy != null)
//        {
//            activeEnemy.AttackPlayer(playerBattle);
//        }

//        // Check if the player is defeated
//        if (playerBattle.CurrentHealth <= 0)
//        {
//            StartCoroutine(EndBattle(false));
//            yield break;
//        }

//        // Update player effects and UI for the next turn
//        playerBattle.DecrementEffectTimers();
//        battleUI.UpdateEffectTimers();

//        // Enable player's turn
//        isPlayerTurn = true;
//        battleUI.UpdateTurnIndicator(isPlayerTurn);
//        battleUI.RenderCards(playerBattle.CardLoadout);
//    }



//    public IEnumerator EndBattle(bool playerWon)
//    {
//        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
//        yield return StartCoroutine(battleUI.ShowBattleResult(playerWon));

//        isPlayerTurn = false;

//        // Transition logic (e.g., load next scene or return to menu)
//        Debug.Log("Transitioning to next scene...");
//        string nextScene = GameManager.Instance.NextScene;
//        levelLoader.LoadScene("BattleScene", nextScene);
//    }
//}

using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerPrefab;
    [SerializeField] private EnemyBattle enemyPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private BattleUI battleUI;

    private PlayerBattle playerInstance;
    private EnemyBattle enemyInstance;
    private bool isPlayerTurn = true;

    private LevelLoader levelLoader;
    //private EnemyBattle activeEnemy;

    private void Start()
    {
        Testing();//run only during development

        levelLoader = FindObjectOfType<LevelLoader>();

        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }

        battleUI.OnStartBattle += HandleStartBattle;
    }

    private void Testing()
    {
        GameManager.Instance.PreviousScene = "ExplorationScene";
        GameManager.Instance.NextScene = "Level 1";
        GameManager.Instance.GameDifficulty = Difficulty.Hard;
        GameManager.Instance.UpdatePlayerHealth(100);
    }

    private void HandleStartBattle()
    {
        // Spawn player and enemy only after clicking "Start Battle"
        SpawnEntities();

        // Initialize the battle after entities spawn
        StartCoroutine(InitializeBattle());
    }

    private void SpawnEntities()
    {
        // Spawn player
        playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        playerInstance.name = "Player";


        if (playerInstance == null)
        {
            Debug.LogError("Player instance failed to spawn!");
            return;
        }

        // Spawn and initialize the enemy
        SpawnCorrectEnemy();
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

        // Update UI before the countdown starts
        battleUI.UpdateEffectTimers();

        yield return StartCoroutine(battleUI.ShowCountdown());

        battleUI.EnableCardInteractions();
        battleUI.RenderCards(playerInstance.CardLoadout);
        battleUI.AddBattleLog($"Battle Start! Enemy: {enemyInstance.EnemyName} with {enemyInstance.CurrentHealth} HP");
        battleUI.UpdateTurnIndicator(isPlayerTurn);

        Debug.Log("Battle initialization complete.");
    }


    private void SpawnCorrectEnemy()
    {
        // Destroy any existing enemy
        if (enemyInstance != null)
        {
            Destroy(enemyInstance.gameObject);
        }

        // Instantiate the enemy prefab
        GameObject enemyObject = Instantiate(enemyPrefab.gameObject, enemySpawnPoint.position, Quaternion.identity);

        // Get the EnemyBattle component
        enemyInstance = enemyObject.GetComponent<EnemyBattle>();
        if (enemyInstance == null)
        {
            Debug.LogError("Enemy prefab does not have an EnemyBattle component!");
            return;
        }

        // Initialize the enemy
        EnemyType enemyType = DetermineEnemyType();
        enemyInstance.Initialize(GameManager.Instance.GameDifficulty, enemyType);

        // Set the GameObject's name to the enemy's name
        enemyObject.name = enemyInstance.EnemyName;

        Debug.Log($"Enemy spawned: {enemyObject.name} with {enemyInstance.MaxHealth} HP.");
    }

    private EnemyType DetermineEnemyType()
    {
        switch (GameManager.Instance.PreviousScene)
        {
            case "ExplorationScene":
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

        // Update effect timers after card usage
        playerInstance.DecrementEffectTimers();
        battleUI.UpdateEffectTimers();

        if (enemyInstance.CurrentHealth <= 0)
        {
            Debug.Log("Enemy defeated!");
            StartCoroutine(EndBattle(true));
            return;
        }

        isPlayerTurn = false;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        StartCoroutine(EnemyTurnWithDelay(selectedCard));
    }

    private IEnumerator EnemyTurnWithDelay(Card selectedCard)
    {
        yield return new WaitForSeconds(1f);

        if (enemyInstance == null)
        {
            Debug.LogError("enemyInstance is null in EnemyTurnWithDelay!");
            yield break;
        }

        if (playerInstance == null)
        {
            Debug.LogError("playerInstance is null in EnemyTurnWithDelay!");
            yield break;
        }

        // Enemy uses a card or action
        enemyInstance.AttackPlayer(playerInstance);

        // Update effect timers for the player after the enemy's turn
        playerInstance.DecrementEffectTimers();
        battleUI.UpdateEffectTimers();

        // Check if the player is defeated
        if (playerInstance.CurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(false));
            yield break;
        }

        // Enable player's turn
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.RenderCards(playerInstance.CardLoadout);
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
    }


    private void SaveBattleResults(bool playerWon)
    {
        // Update the player's statistics
        if (playerWon)
        {
            GameManager.Instance.AddMiniBattleWin(); // Example: Increment mini-battle wins
        }
        else
        {
            GameManager.Instance.AddMiniBattleLoss(); // Increment mini-battle losses
        }

        // Save the player's coins
        GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.CurrentCoins1 + 10); // adding only 10 for now but will have a varied way to develop it.

        // Save the enemy's defeat
        if (playerWon)
        {
            string defeatedEnemy = enemyInstance.EnemyName;
            GameManager.Instance.CompleteObjective($"Defeated {defeatedEnemy}");
        }

        // Save any additional data, like if player unlocked a new card
        GameManager.Instance.UpdatePlayerCards(playerInstance.CardLoadout.Select(c => c.Name).ToArray());

        // Log save operation
        Debug.Log("Battle results saved to GameManager.");
    }

}
