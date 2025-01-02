using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class BattleEntity : MonoBehaviour
{
    protected Animator animator;
    //public Animator Animator => animator;

    protected int maxHealth;
    protected int currentHealth;

    protected int baseDefence = 0;
    protected int currentDefence;

    protected Slider healthBar;

    protected List<Card> cardLoadout = new List<Card>();
    protected List<(int value, int timer)> temporaryDefenses = new List<(int, int)>();
    protected List<(int value, int timer)> temporaryHeals = new List<(int, int)>();

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentDefence => currentDefence;
    public List<(int value, int timer)> TemporaryDefences => temporaryDefenses;
    public List<(int value, int timer)> TemporaryHeals => temporaryHeals;
    public List<Card> CardLoadout => cardLoadout;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        currentDefence = baseDefence;
        //healthBar = GetComponent<Slider>();

        if(healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            UpdateHealthBar();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        int netDamage = Mathf.Max(damageAmount - currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - netDamage, 0);

        Debug.Log($"{name}: Damage Taken: {damageAmount}, Current Health: {currentHealth}, Defence: {currentDefence}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"Player healed by {healAmount}, Current Health: {currentHealth}");
        UpdateHealthBar(); 
    }



    public void AddDefence(int defenceAmount)
    {
        currentDefence += defenceAmount;
        Debug.Log($"{name}'s defence increased by {defenceAmount}. Current defence: {currentDefence}");
    }

    public void ResetDefence()
    {
        currentDefence = baseDefence;
        Debug.Log($"{name}'s defence reset to base.");
    }

    public void AddTemporaryDefence(int value, int timer)
    {
        temporaryDefenses.Add((value, timer));
        UpdateCurrentDefence();
    }

    public void AddTemporaryHealing(int value, int timer)
    {
        temporaryHeals.Add((value, timer));
    }

    protected void UpdateCurrentDefence()
    {
        currentDefence = baseDefence + temporaryDefenses.Sum(d => d.value);
    }

    public void DecrementEffectTimers()
    {
        // Decrement defense timers
        for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
        {
            temporaryDefenses[i] = (temporaryDefenses[i].value, temporaryDefenses[i].timer - 1);
            if (temporaryDefenses[i].timer <= 0)
            {
                temporaryDefenses.RemoveAt(i);
            }
        }
        UpdateCurrentDefence();

        // Decrement healing timers
        for (int i = temporaryHeals.Count - 1; i >= 0; i--)
        {
            Heal(temporaryHeals[i].value);
            temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
            if (temporaryHeals[i].timer <= 0)
            {
                temporaryHeals.RemoveAt(i);
            }
        }
    }

    // Updates the health of the entity
    protected void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{name} has been defeated.");
        // Add death logic here, e.g., animations or scene transitions
    }

}
