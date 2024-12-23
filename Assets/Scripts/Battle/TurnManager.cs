using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public Player player; // Reference to the player
    public Enemy enemy;   // Reference to the enemy
    private bool isPlayerTurn = true;

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log("Player's turn. Choose a card to play.");
            // Enable card selection UI for the player
        }
        else
        {
            Debug.Log("Enemy's turn.");
            EnemyTurn();
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        StartTurn();
    }

    private void EnemyTurn()
    {
        // Simulate enemy action (temporary)
        enemy.Attack(player);
        EndTurn();
    }
}
