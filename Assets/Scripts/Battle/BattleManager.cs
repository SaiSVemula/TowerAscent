using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerBattle;
    private EnemyBattle activeEnemy; // The actual enemy used in battle
    [SerializeField] private Transform enemySpawnPoint; // Assign the position where the enemy will spawn
    [SerializeField] private Enemy1 enemy1Prefab;
    [SerializeField] private Enemy2 enemy2Prefab;
    [SerializeField] private Enemy3 enemy3Prefab;
    [SerializeField] private BattleUI battleUI;

    private bool isPlayerTurn = true;

    private LevelLoader levelLoader;

    private void Start()
    {
        GameManager.Instance.PreviousScene = "ExplorationScene";
        GameManager.Instance.NextScene = "Level 1";
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

        Debug.Log("Battle initialization started");

        // Spawn the correct enemy based on the PreviousScene
        SpawnCorrectEnemy();

        if (activeEnemy == null)
        {
            Debug.LogError("Enemy spawning failed!");
            yield break;
        }

        yield return StartCoroutine(battleUI.ShowCountdown());

        Debug.Log("Battle initialization in progress");

        battleUI.RenderCards(playerBattle.CardLoadout);
        battleUI.EnableCardInteractions();
        battleUI.AddBattleLog($"Battle Start! Enemy: {activeEnemy.EnemyName} with {activeEnemy.CurrentHealth} HP");
        battleUI.UpdateTurnIndicator(true);

        Debug.Log("Battle initialization complete");
    }

    private void SpawnCorrectEnemy()
    {
        string previousScene = GameManager.Instance.PreviousScene;
        EnemyBattle spawnedEnemy = null;

        switch (previousScene)
        {
            case "ExplorationScene":
                spawnedEnemy = Instantiate(enemy1Prefab, enemySpawnPoint.position, Quaternion.identity);
                break;
            case "Level 1":
                spawnedEnemy = Instantiate(enemy2Prefab, enemySpawnPoint.position, Quaternion.identity);
                break;
            case "Level 2":
                spawnedEnemy = Instantiate(enemy3Prefab, enemySpawnPoint.position, Quaternion.identity);
                break;
            default:
                Debug.LogError("Invalid PreviousScene value!");
                return;
        }

        if (spawnedEnemy != null)
        {
            activeEnemy = spawnedEnemy; // Set the active enemy in BattleManager
            activeEnemy.Initialize(GameManager.Instance.GameDifficulty);

            // Notify BattleUI of the active enemy
            if (battleUI != null)
            {
                battleUI.SetActiveEnemy(activeEnemy);
            }
            else
            {
                Debug.LogError("BattleUI reference missing!");
            }
        }
        else
        {
            Debug.LogError("Failed to spawn enemy!");
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
        battleUI.DisableCardInteractions();
        StartCoroutine(EnemyTurnWithDelay());
    }

    private IEnumerator EnemyTurnWithDelay()
    {
        yield return new WaitForSeconds(1f); // Add a delay before enemy action

        Debug.Log($"{activeEnemy.EnemyName} attacks the player!");
        playerBattle.TakeDamage(10); // Placeholder damage for the attack

        // Check if player is defeated
        if (playerBattle.CurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(false));
            yield break;
        }

        // Update player effects and UI for the next turn
        playerBattle.DecrementEffectTimers();
        battleUI.UpdateEffectTimers();
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.RenderCards(playerBattle.CardLoadout);
        battleUI.EnableCardInteractions();
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
