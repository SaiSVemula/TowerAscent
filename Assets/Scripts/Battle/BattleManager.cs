using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public Player player; // Reference to the player
    public Enemy enemy;   // Reference to the enemy
    public Transform playerHandTransform; // Transform to hold player cards in the scene

    private bool isPlayerTurn;

    void Start()
    {
        InitializeBattle();
    }

    // Initializes the battle
    void InitializeBattle()
    {
        Debug.Log("Battle Start!");

        // Initialize player and enemy stats
        player = new Player("Hero", 100, 0);
        enemy = new Enemy { Name = "Goblin", Health = 50, AttackDamage = 10 };

        // Log player and enemy stats
        Debug.Log($"Player: {player.Name}, Health: {player.Health}");
        Debug.Log($"Enemy: {enemy.Name}, Health: {enemy.Health}");

        // Start with the player's turn
        isPlayerTurn = true;
        StartCoroutine(HandleTurn());
    }

    // Handles the turn logic
    IEnumerator HandleTurn()
    {
        while (player.Health > 0 && enemy.Health > 0)
        {
            if (isPlayerTurn)
            {
                Debug.Log("Player's Turn!");
                yield return HandlePlayerTurn();
            }
            else
            {
                Debug.Log("Enemy's Turn!");
                yield return HandleEnemyTurn();
            }

            // Alternate turns
            isPlayerTurn = !isPlayerTurn;
        }

        EndBattle();
    }

    // Handles the player's turn
    IEnumerator HandlePlayerTurn()
    {
        Debug.Log("Waiting for the player to play a card...");

        bool cardPlayed = false;

        while (!cardPlayed)
        {
            // Check for card interactions (e.g., clicking or dragging cards)
            foreach (Transform cardTransform in playerHandTransform)
            {
                var card = cardTransform.GetComponent<Card>();
                if (card != null && card.IsClicked()) // Replace `IsClicked` with your card interaction logic
                {
                    Debug.Log($"Card {card.Name} played.");
                    card.Use(player, enemy); // Apply card's effect
                    cardPlayed = true;
                    break;
                }
            }

            yield return null; // Wait for the next frame
        }

        // Log the player's status after their turn
        player.PrintStatus();
    }

    // Handles the enemy's turn
    IEnumerator HandleEnemyTurn()
    {
        yield return new WaitForSeconds(1f); // Simulate enemy "thinking"

        Debug.Log("Enemy attacks the player!");
        enemy.Attack(player);

        // Log the player's status after the enemy's attack
        player.PrintStatus();

        yield return null;
    }

    // Ends the battle and determines the winner
    void EndBattle()
    {
        if (player.Health <= 0)
        {
            Debug.Log("You lost the battle!");
        }
        else if (enemy.Health <= 0)
        {
            Debug.Log("You won the battle!");
        }
    }
}
