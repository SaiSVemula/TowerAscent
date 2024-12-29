// BattleManager.cs
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerBattle;
    [SerializeField] private EnemyBattle enemyBattle;
    [SerializeField] private BattleUI battleUI;

    private bool isPlayerTurn = true;

    public IEnumerator InitializeBattle()
    {
        
        Debug.Log("Battle initialization started");
        

        if (battleUI == null || playerBattle == null)
        {
            Debug.LogError("Critical references missing!");
            yield break;
        }

        yield return StartCoroutine(battleUI.ShowCountdown());

        Debug.Log("Battle initialization in progress");

        battleUI.RenderCards(playerBattle.PlayerCardLoadout);
        battleUI.EnableCardInteractions();
        battleUI.AddBattleLog("Battle Start!");
        battleUI.UpdateTurnIndicator(true);

        Debug.Log("Battle initialization complete");
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
        yield return new WaitForSeconds(1f); // Wait before enemy action

        enemyBattle.EnemyAttackPlayer(playerBattle);

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

        //transition into next scene or back into the main scene
    }
}
