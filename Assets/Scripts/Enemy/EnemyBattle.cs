using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class EnemyBattle : MonoBehaviour
{
    [SerializeField] protected string enemyName = "Enemy";
    [SerializeField] protected int enemyMaxHealth;
    protected int enemyCurrentHealth;

    [SerializeField] protected List<Card> enemyCardLoadout = new List<Card>();
    [SerializeField] private Slider enemyHealthBar;

    public string EnemyName => enemyName;
    public int EnemyCurrentHealth => enemyCurrentHealth;

    public void EnemyTakeDamage(int damageAmount)
    {
        enemyCurrentHealth = Mathf.Max(enemyCurrentHealth - damageAmount, 0);
        UpdateHealthBar();
        Debug.Log($"{enemyName} takes {damageAmount} damage. Current health: {enemyCurrentHealth}");
    }

    public void UpdateHealthBar()
    {
        if (enemyHealthBar != null)
        {
            enemyHealthBar.maxValue = enemyMaxHealth;
            enemyHealthBar.value = enemyCurrentHealth;
        }
    }

    public abstract void AttackPlayer(PlayerBattle player);

    public abstract void Initialize(Difficulty difficulty);

    protected void LogEnemyInfo()
    {
        Debug.Log($"Initialized Enemy: {enemyName}");
        Debug.Log($"Max Health: {enemyMaxHealth}");
        Debug.Log($"Cards in Loadout:");
        foreach (var card in enemyCardLoadout)
        {
            Debug.Log($"- {card.Name}");
        }
    }
}
