using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerBattle;
    private EnemyBattle enemyBattle;
    [SerializeField] private BattleUI battleUI;
    private bool isPlayerTurn = true;

    private void Start()
    {
        StartCoroutine(InitializeBattle());
    }

    public IEnumerator InitializeBattle()
    {
        if (battleUI == null || playerBattle == null || GameManager.Instance == null)
        {
            Debug.LogError("Critical references missing!");
            yield break;
        }

        Debug.Log("Battle initialization started");

        LoadEnemyFromGameManager();



        if (enemyBattle == null)
        {
            Debug.LogError("Enemy loading failed!");
            yield break;
        }

        yield return StartCoroutine(battleUI.ShowCountdown());

        Debug.Log("Battle initialization in progress");

        battleUI.RenderCards(playerBattle.PlayerCardLoadout);
        battleUI.EnableCardInteractions();
        battleUI.AddBattleLog($"Battle Start! Enemy: {enemyBattle.EnemyName} with {enemyBattle.EnemyCurrentHealth} HP");
        battleUI.UpdateTurnIndicator(true);

        Debug.Log("Battle initialization complete");
    }

    private void LoadEnemyFromGameManager()
    {
        string previousScene = GameManager.Instance.PreviousScene;
        Difficulty difficulty = GameManager.Instance.GameDifficulty;

        Debug.Log($"Loading enemy based on previous scene: {previousScene}, Difficulty: {difficulty}");

        switch (previousScene)
        {
            case "ExplorationScene":
                enemyBattle = Instantiate(Resources.Load<Enemy1>("Enemies/Enemy1"));
                break;
            case "Level 1":
                enemyBattle = Instantiate(Resources.Load<Enemy2>("Enemies/Enemy2"));
                break;
            case "Level 2":
                enemyBattle = Instantiate(Resources.Load<Enemy3>("Enemies/Enemy3"));
                break;
            default:
                Debug.LogError("Invalid PreviousScene value in GameManager!");
                return;
        }

        enemyBattle.Initialize(difficulty);
    }

    public void OnPlayerUseCard(int cardIndex)
    {
        if (!isPlayerTurn) return;

        Card selectedCard = playerBattle.PlayerCardLoadout[cardIndex];
        string logMessage = selectedCard.Use(playerBattle, enemyBattle);
        battleUI.AddBattleLog(logMessage);

        if (enemyBattle.EnemyCurrentHealth <= 0)
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
        yield return new WaitForSeconds(1f);

        enemyBattle.AttackPlayer(playerBattle);

        if (playerBattle.PlayerCurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(false));
            yield break;
        }

        playerBattle.DecrementEffectTimers();
        battleUI.UpdateEffectTimers();
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.RenderCards(playerBattle.PlayerCardLoadout);
    }

    public IEnumerator EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
        yield return StartCoroutine(battleUI.ShowBattleResult(playerWon));

        isPlayerTurn = false;
    }
}
