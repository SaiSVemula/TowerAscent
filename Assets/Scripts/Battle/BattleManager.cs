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
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerBattle;
    private EnemyBattle activeEnemy; // The actual enemy used in battle
    [SerializeField] private Transform enemySpawnPoint; // Assign the position where the enemy will spawn
    [SerializeField] private EnemyBattle enemyBattle; // Single prefab for EnemyBattle
    [SerializeField] private BattleUI battleUI;

    private bool isPlayerTurn = true;

    private LevelLoader levelLoader;

    private void Start()
    {
        GameManager.Instance.PreviousScene = "ExplorationScene";
        GameManager.Instance.NextScene = "Level 1";
        GameManager.Instance.GameDifficulty = Difficulty.Hard;
        levelLoader = FindObjectOfType<LevelLoader>();

        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
        StartCoroutine(InitializeBattle());
    }

    public IEnumerator InitializeBattle()
    {
        if (battleUI == null || playerBattle == null)
        {
            Debug.LogError("Critical references missing!");
            yield break;
        }

        Debug.Log("Waiting for instructions to start battle...");

        // Wait for instruction canvas to be completed
        yield return new WaitUntil(() => battleUI.IsBattleReady);

        Debug.Log("Instruction canvas completed, starting battle initialization...");

        // Spawn the correct enemy
        SpawnCorrectEnemy();

        if (activeEnemy == null)
        {
            Debug.LogError("Enemy spawning failed!");
            yield break;
        }

        yield return StartCoroutine(battleUI.ShowCountdown());

        battleUI.RenderCards(playerBattle.CardLoadout);
        battleUI.EnableCardInteractions();
        battleUI.AddBattleLog($"Battle Start! Enemy: {activeEnemy.EnemyName} with {activeEnemy.CurrentHealth} HP");
        battleUI.UpdateTurnIndicator(true);

        Debug.Log("Battle initialization complete");
    }

    private void SpawnCorrectEnemy()
    {
        // Destroy any existing enemy in the scene
        if (activeEnemy != null)
        {
            Destroy(activeEnemy.gameObject);
        }

        // Instantiate the correct enemy
        GameObject enemyObject = Instantiate(enemyBattle.gameObject, enemySpawnPoint.position, Quaternion.identity);
        enemyObject.SetActive(true); 
        activeEnemy = enemyObject.GetComponent<EnemyBattle>();

        if (activeEnemy == null)
        {
            Debug.LogError("Enemy prefab does not have an EnemyBattle component!");
            return;
        }

        // Initialize enemy
        EnemyType enemyType = DetermineEnemyType();
        activeEnemy.Initialize(GameManager.Instance.GameDifficulty, enemyType);

        // Set the enemy's rotation to face the player
        enemyObject.transform.LookAt(playerBattle.transform.position);

        // Notify the UI
        if (battleUI != null)
        {
            battleUI.SetActiveEnemy(activeEnemy);
        }
        else
        {
            Debug.LogError("BattleUI reference is missing!");
        }

        Debug.Log($"Enemy {activeEnemy.EnemyName} activated for battle.");
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

        if (cardIndex < 0 || cardIndex >= playerBattle.CardLoadout.Count)
        {
            Debug.LogError("Invalid card index!");
            return;
        }

        Card selectedCard = playerBattle.CardLoadout[cardIndex];
        if (selectedCard == null)
        {
            Debug.LogError("Selected card is null!");
            return;
        }

        string logMessage = selectedCard.Use(playerBattle, activeEnemy);
        battleUI.AddBattleLog(logMessage);

        if (activeEnemy.CurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(true));
            return;
        }

        battleUI.UpdateEffectTimers();
        isPlayerTurn = false;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        StartCoroutine(EnemyTurnWithDelay());
    }

    private IEnumerator EnemyTurnWithDelay()
    {
        yield return new WaitForSeconds(1f); // Add a delay before enemy action

        // Let the enemy attack the player
        if (activeEnemy != null)
        {
            activeEnemy.AttackPlayer(playerBattle);
        }

        // Check if the player is defeated
        if (playerBattle.CurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(false));
            yield break;
        }

        // Update player effects and UI for the next turn
        playerBattle.DecrementEffectTimers();
        battleUI.UpdateEffectTimers();

        // Enable player's turn
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.RenderCards(playerBattle.CardLoadout);
    }

    public IEnumerator EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
        yield return StartCoroutine(battleUI.ShowBattleResult(playerWon));

        isPlayerTurn = false;

        // Transition logic (e.g., load next scene or return to menu)
        Debug.Log("Transitioning to next scene...");
        string nextScene = GameManager.Instance.NextScene;
        levelLoader.LoadScene("BattleScene", nextScene);
    }
}
