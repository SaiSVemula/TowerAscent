// PlayerBattle.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private int baseDefence = 0;
    private int currentDefence;

    [SerializeField] private List<Card> cardLoadout;

    [SerializeField] private Slider healthBarSlider;
    public int CurrentHealth => currentHealth;
    public int CurrentDefence => currentDefence;
    public List<Card> CardLoadout { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        currentDefence = baseDefence;

        healthBarSlider.maxValue = maxHealth;
        UpdateHealthBar();
        //Debug.Log($"Current health: {currentHealth}");

        //UpdateHealthBar();

        CardLoadout = new List<Card>
        {
            Resources.Load<Card>("Cards/Axe Chop"),
            Resources.Load<Card>("Cards/Fireball"),
            Resources.Load<Card>("Cards/Dodge"),
            Resources.Load<Card>("Cards/First Aid")
        };
    }

    // method to update the health bar fill amount
    public void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int amount)
    {
        int netDamage = Mathf.Max(amount - currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - netDamage, 0);
        //UpdateHealthBar();
        Debug.Log($"Player takes {netDamage} damage. Current health: {currentHealth}");
    }

    public void AddDefence(int amount)
    {
        currentDefence += amount;
        Debug.Log($"Player defence increased by {amount}. Current defence: {currentDefence}");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        //UpdateHealthBar();
        Debug.Log($"Player heals {amount}. Current health: {currentHealth}");
    }

    public void ResetDefence()
    {
        currentDefence = baseDefence;
        Debug.Log("Player defence reset to base.");
    }

    public void UseCard(int cardIndex, EnemyBattle targetEnemy)
    {
        if (cardIndex < 0 || cardIndex >= cardLoadout.Count) return;

        Card selectedCard = cardLoadout[cardIndex];
        if (selectedCard != null)
        {
            selectedCard.Use(this, targetEnemy);
        }
    }
}