// EnemyBattle.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBattle : MonoBehaviour
{
    [SerializeField] private string enemyName = "Enemy";
    [SerializeField] private int maxHealth = 50;// change this according to the enemy
    private int currentHealth;

    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Slider healthBarSlider;

    public string EnemyName => enemyName;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        // Initialize the enemy's health
        currentHealth = maxHealth;
        healthBarSlider.maxValue = maxHealth;
        Debug.Log($"Current health: {currentHealth}");
        UpdateHealthBar();
    }

    // Update the health bar fill amount
    public void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        //UpdateHealthBar();
        Debug.Log($"{enemyName} takes {amount} damage. Current health: {currentHealth}");
    }

    public void AttackPlayer(PlayerBattle player)
    {
        Debug.Log($"{enemyName} attacks the player for {attackDamage} damage!");
        player.TakeDamage(attackDamage);
    }
}