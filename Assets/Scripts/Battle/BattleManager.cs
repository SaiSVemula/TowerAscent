using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        //Testing();//run only during development

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
        GameManager.Instance.GameDifficulty = Difficulty.Easy;
        GameManager.Instance.UpdatePlayerHealth(100);

        List<Card> cardLoadout = new List<Card>
        {
        Resources.Load<Card>("Cards/Weapon Cards/Axe Chop"),
        Resources.Load<Card>("Cards/Magic Cards/Fireball"),
        Resources.Load<Card>("Cards/Defence Cards/Dodge"),
        Resources.Load<Card>("Cards/Healing Cards/First Aid")
        };

        GameManager.Instance.CurrentCardLoadout = cardLoadout;  
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
        //enemyInstance.Initialize(GameManager.Instance.GameDifficulty, enemyType);
        int difficulty = PlayerPrefs.GetInt("PlayersGameDifficulty", 0);
        enemyInstance.Initialize(difficulty, enemyType);

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

        // Update effect timers for the enemy after its turn
        enemyInstance.DecrementEffectTimers();
        Debug.Log("Enemy effect timers decremented.");

        // Update UI to reflect enemy effect timers (if applicable)
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
        //levelLoader.LoadScene("BattleScene", nextScene);
        levelLoader.LoadScene("BattleScene", "LevelTransitionCutScene");
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
        GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.CurrentCoins1 + 10); // adding only 10 for now but will have a varied way to develop it.

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
