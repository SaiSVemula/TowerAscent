using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattle : MonoBehaviour
{
    [SerializeField] private string enemyName = "Enemy";
    [SerializeField] private int enemyMaxHealth = 50;
    private int enemyCurrentHealth;

    [SerializeField] private int enemyAttackDamage = 10;
    [SerializeField] private Slider enemyHealthBar; // Enemy-specific health bar

    public string EnemyName => enemyName;
    public int EnemyCurrentHealth => enemyCurrentHealth;

    private void Awake()
    {
        enemyCurrentHealth = enemyMaxHealth;
        enemyHealthBar.maxValue = enemyMaxHealth;
        UpdateEnemyHealthBar();
    }

    private void UpdateEnemyHealthBar()
    {
        if (enemyHealthBar != null)
        {
            enemyHealthBar.value = enemyCurrentHealth;
        }
    }

    public void EnemyTakeDamage(int damageAmount)
    {
        enemyCurrentHealth = Mathf.Max(enemyCurrentHealth - damageAmount, 0);
        UpdateEnemyHealthBar();
        Debug.Log($"{enemyName} takes {damageAmount} damage. Current health: {enemyCurrentHealth}");
    }

    public void EnemyAttackPlayer(PlayerBattle player)
    {
        Debug.Log($"{enemyName} attacks the player for {enemyAttackDamage} damage!");
        player.PlayerTakeDamage(enemyAttackDamage);
    }
}
