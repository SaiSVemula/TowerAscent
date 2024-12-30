using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    //protected Animator animator;

    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    [SerializeField] protected int baseDefence = 0;
    protected int currentDefence;

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
        //animator.SetBool("InBattle", true);
        currentHealth = maxHealth;
        currentDefence = baseDefence;
    }

    public abstract void UseCard(int cardIndex, BattleEntity target);

    public void TakeDamage(int damageAmount)
    {
        int netDamage = Mathf.Max(damageAmount - currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - netDamage, 0);
        //animator.SetTrigger("GetHit");
        Debug.Log($"{name} takes {netDamage} damage. Current health: {currentHealth}");
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"{name} heals {healAmount} HP. Current health: {currentHealth}");
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
}
