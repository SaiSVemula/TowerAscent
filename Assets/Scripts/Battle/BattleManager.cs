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
        battleUI.RenderCards(playerBattle.PlayerCardLoadout);
    }

    public void StartEnemyTurn()
    {
        if (!isPlayerTurn) return;

        isPlayerTurn = false;

        // Enemy attacks
        enemyBattle.EnemyAttackPlayer(playerBattle);

        // Update health bar
        playerBattle.UpdatePlayerHealthBar();

        // Check for loss condition
        if (playerBattle.PlayerCurrentHealth <= 0)
        {
            EndBattle(false);
            return;
        }

        // Return to player's turn
        isPlayerTurn = true;
    }


    public void OnPlayerUseCard(int cardIndex)
    {
        Debug.Log($"Card index {cardIndex} clicked");
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

        // Switch to enemy turn
        isPlayerTurn = false;
        enemyBattle.UpdateEnemyHealthBar();
        EnemyTurn();
    }


    private void EnemyTurn()
    {
        enemyBattle.EnemyAttackPlayer(playerBattle);

        // Check if the player is defeated
        if (playerBattle.PlayerCurrentHealth <= 0)
        {
            EndBattle(false);
            return;
        }

        // Return to player's turn
        playerBattle.UpdatePlayerHealthBar();
        isPlayerTurn = true;
    }

    public void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
        // Implement end game logic here (e.g., show results UI)
    }
}
