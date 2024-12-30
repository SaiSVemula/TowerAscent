using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected string enemyName = "Enemy";
    [SerializeField] protected int enemyMaxHealth;
    protected int enemyCurrentHealth;

    [SerializeField] protected List<Card> enemyCardLoadout = new List<Card>();

    public string EnemyName => enemyName;
    public int EnemyCurrentHealth => enemyCurrentHealth;

    public void TakeDamage(int damageAmount)
    {
        enemyCurrentHealth = Mathf.Max(enemyCurrentHealth - damageAmount, 0);
        Debug.Log($"{enemyName} takes {damageAmount} damage. Current health: {enemyCurrentHealth}");
    }

    public void AttackPlayer(PlayerBattle player)
    {
        Debug.Log($"{enemyName} attacks the player!");
        // Define common attack behavior here or make it abstract if each enemy attacks differently.
    }

    public abstract void Initialize(EnemyDifficulty difficulty);
}
