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
        battleUI.RenderCards(playerBattle.CardLoadout);
    }

    public void OnPlayerUseCard(int cardIndex)
    {
        Debug.Log($"Card index {cardIndex} clicked");//debug
        if (!isPlayerTurn) return;

        // Use the selected card
        Card selectedCard = playerBattle.CardLoadout[cardIndex];
        selectedCard.Use(playerBattle, enemyBattle);

        // Check if the enemy is defeated
        if (enemyBattle.CurrentHealth <= 0)
        {
            EndBattle(true);
            return;
        }

        // Switch to enemy turn
        isPlayerTurn = false;
        enemyBattle.UpdateHealthBar();
        EnemyTurn();
    }

    private void EnemyTurn()
    {
        enemyBattle.AttackPlayer(playerBattle);

        // Check if the player is defeated
        if (playerBattle.CurrentHealth <= 0)
        {
            EndBattle(false);
            return;
        }

        // Return to player's turn
        playerBattle.UpdateHealthBar();
        isPlayerTurn = true;
    }

    private void EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
        // Implement end game logic here (e.g., show results UI)
    }
}
