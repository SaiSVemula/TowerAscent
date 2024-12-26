// BattleManager.cs
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerBattle;
    [SerializeField] private EnemyBattle enemyBattle;
    [SerializeField] private BattleUI battleUI;

    private bool isPlayerTurn = true;

    private void Start()
    {
        // Render initial cards for the player
        battleUI.RenderCards(playerBattle.PlayerCardLoadout);
    }

    public void OnPlayerUseCard(int cardIndex)
    {
        if (!isPlayerTurn) return;

        // Use the selected card
        Card selectedCard = playerBattle.PlayerCardLoadout[cardIndex];
        selectedCard.Use(playerBattle, enemyBattle);

        // Check if the enemy is defeated
        if (enemyBattle.EnemyCurrentHealth <= 0)
        {
            EndBattle(true);
            return;
        }

        // Decrement timers and switch to the enemy's turn
        battleUI.UpdateEffectTimers(); // Update UI
        isPlayerTurn = false;
        EnemyTurn();
    }

    public void EnemyTurn()
    {
        // Enemy attacks the player
        enemyBattle.EnemyAttackPlayer(playerBattle);

        // Check if the player is defeated
        if (playerBattle.PlayerCurrentHealth <= 0)
        {
            EndBattle(false);
            return;
        }

        // Decrement timers and switch to the player's turn
        playerBattle.DecrementEffectTimers();
        battleUI.UpdateEffectTimers(); // Update UI
        isPlayerTurn = true;
        battleUI.RenderCards(playerBattle.PlayerCardLoadout);
    }

    public void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
        // Implement end game logic here (e.g., show results UI)
    }
}
