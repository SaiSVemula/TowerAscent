using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBattle : BattleEntity
{
    [SerializeField] private Slider PlayerHealthBar;
    // Fields for mini-battle
    private List<Card> miniBattleCardPool = new List<Card>();


    protected override void Awake()
    {
        Debug.Log("PlayerBattle Awake called");
        // Clear existing card loadout to avoid duplicates
        //LoadManager.LoadGameState();
        cardLoadout.Clear();

        // Initialize base class properties
        base.Awake();

        // Initialize player health
        maxHealth = GameManager.Instance.GetPlayerHealth();

        if (maxHealth == 0)
        {
            maxHealth = 100;
        }

        Debug.Log($"Base maxHealth: {maxHealth}");

        healthBar = PlayerHealthBar;
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            UpdateHealthBar();
        }

        // Add logic to load mini-battle features only outside the BattleScene
        if (SceneManager.GetActiveScene().name != "BattleScene")
        {
            Debug.Log("Setting up mini-battle features for the player.");
            LoadMiniBattleCardPool();
        }
        else
        {
            // Initialize card loadout with default cards
            cardLoadout = GameManager.Instance.CurrentCardLoadout;

            // Log the card loadout initialization
            Debug.Log($"Card loadout initialized with {cardLoadout.Count} cards.");
        }
    }

    public void UsePlayerCard(int cardIndex, EnemyBattle targetEnemy)
    {
        if (cardIndex < 0 || cardIndex >= cardLoadout.Count)
        {
            return;
        }

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

    //public void DecrementEffectTimers()
    //{
    //    // Ensure temporary lists are initialized
    //    if (temporaryDefenses == null)
    //    {
    //        temporaryDefenses = new List<(int, int)>();
    //    }
    //    if (temporaryHeals == null)
    //    {
    //        temporaryHeals = new List<(int, int)>();
    //    }
    //    // Decrement defense timers
    //    for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
    //    {
    //        temporaryDefenses[i] = (temporaryDefenses[i].value, temporaryDefenses[i].timer - 1);
    //        if (temporaryDefenses[i].timer <= 0)
    //        {
    //            Debug.Log($"Defence effect expired: {temporaryDefenses[i].value}");
    //            temporaryDefenses.RemoveAt(i);
    //        }
    //    }
    //    UpdateCurrentDefence();

    //    // Decrement healing timers
    //    for (int i = temporaryHeals.Count - 1; i >= 0; i--)
    //    {
    //        Heal(temporaryHeals[i].value); // Apply healing
    //        temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
    //        if (temporaryHeals[i].timer <= 0)
    //        {
    //            Debug.Log($"Healing effect expired: {temporaryHeals[i].value}");
    //            temporaryHeals.RemoveAt(i);
    //        }
    //    }
    //}

    public override void DecrementEffectTimers()
    {
        base.DecrementEffectTimers(); // Call base logic for decrementing

        // Refresh the UI
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            BattleUI battleUI = FindObjectOfType<BattleUI>();
            UpdateHealthBar();
        }

        Debug.Log("Player timers decremented and UI updated.");
    }


    protected override void OnEffectTimersUpdated()
    {
        // Add any player-specific logic if needed
        Debug.Log("Player-specific logic after decrementing effect timers.");
    }

    public void SetUpMiniBattle()
    {
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            return;
        }

        Debug.Log("Setting up mini-battle stats for the player.");
        LoadMiniBattleCardPool();

        // Reset player health for the mini-battle
        maxHealth = GameManager.Instance.GetPlayerHealth();
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    public void LoadMiniBattleCardPool()
    {
        Debug.Log("Loading mini-battle card pool...");

        // Fetch the pool from the GameManager
        miniBattleCardPool = GameManager.Instance.GetMiniBattleCardPool();

        if (miniBattleCardPool == null || miniBattleCardPool.Count == 0)
        {
            Debug.LogWarning("No cards available in MiniBattleCardPool. Adding default card.");
            GameManager.Instance.AddDefaultCardIfPoolEmpty();
            miniBattleCardPool = GameManager.Instance.GetMiniBattleCardPool();
        }

        Debug.Log($"Mini-battle card pool loaded with {miniBattleCardPool.Count} cards.");
    }

    public Card GetMiniBattleCard()
    {
        if (miniBattleCardPool == null || miniBattleCardPool.Count == 0)
        {
            Debug.LogWarning("No cards available in MiniBattleCardPool. Fetching default pool.");
            GameManager.Instance.AddDefaultCardIfPoolEmpty();
            miniBattleCardPool = GameManager.Instance.GetMiniBattleCardPool();
        }

        if (miniBattleCardPool == null || miniBattleCardPool.Count == 0)
        {
            Debug.LogError("MiniBattleCardPool is still empty. No cards to fetch.");
            return null;
        }

        // Select a random card (or implement specific logic for choosing a card)
        int randomIndex = Random.Range(0, miniBattleCardPool.Count);
        Debug.Log($"Returning card: {miniBattleCardPool[randomIndex].Name}");
        return miniBattleCardPool[randomIndex];
    }

}