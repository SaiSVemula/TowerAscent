using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : BattleEntity
{
    [SerializeField] private Slider PlayerHealthBar;
    protected override void Awake()
    {
        Debug.Log("PlayerBattle Awake called");
        // Clear existing card loadout to avoid duplicates
        cardLoadout.Clear();

        maxHealth = GameManager.Instance.GetPlayerHealth();

        // Initialize base class properties
        base.Awake();

        // Log maxHealth for debugging
        Debug.Log($"Base maxHealth: {maxHealth}");

        healthBar = PlayerHealthBar;
        currentHealth = maxHealth; // Initialize current health
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth; // Sync max value
            UpdateHealthBar(); // Set initial value
        }

        // Initialize card loadout with default cards
        cardLoadout = GameManager.Instance.CurrentCardLoadout;

        // Log the card loadout initialization
        Debug.Log($"Card loadout initialized with {cardLoadout.Count} cards.");
    }

    public void UsePlayerCard(int cardIndex, EnemyBattle targetEnemy)
    {
        if (cardIndex < 0 || cardIndex >= cardLoadout.Count) return;

        Card selectedCard = cardLoadout[cardIndex];
        if (selectedCard != null)
        {
            Debug.Log($"Player used {selectedCard.Name} on {targetEnemy.name}");
            Debug.Log(selectedCard.Use(this, targetEnemy));
        }
    }

    public void UpdateEffectTimers()
    {
        Debug.Log("Updating effect timers for PlayerBattle");

        // Update defence text
        if (temporaryDefenses.Count > 0)
        {
            Debug.Log($"Active defences: {string.Join(", ", temporaryDefenses.Select(d => $"{d.value} ({d.timer} turns)"))}");
        }

        // Update healing text
        if (temporaryHeals.Count > 0)
        {
            Debug.Log($"Active healing: {string.Join(", ", temporaryHeals.Select(h => $"{h.value} ({h.timer} turns)"))}");
        }
    }

    public void DecrementEffectTimers()
    {
        // Ensure temporary lists are initialized
        if (temporaryDefenses == null) temporaryDefenses = new List<(int, int)>();
        if (temporaryHeals == null) temporaryHeals = new List<(int, int)>();

        // Decrement defense timers
        for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
        {
            temporaryDefenses[i] = (temporaryDefenses[i].value, temporaryDefenses[i].timer - 1);
            if (temporaryDefenses[i].timer <= 0)
            {
                Debug.Log($"Defence effect expired: {temporaryDefenses[i].value}");
                temporaryDefenses.RemoveAt(i);
            }
        }
        UpdateCurrentDefence();

        // Decrement healing timers
        for (int i = temporaryHeals.Count - 1; i >= 0; i--)
        {
            Heal(temporaryHeals[i].value); // Apply healing
            temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
            if (temporaryHeals[i].timer <= 0)
            {
                Debug.Log($"Healing effect expired: {temporaryHeals[i].value}");
                temporaryHeals.RemoveAt(i);
            }
        }
    }
}