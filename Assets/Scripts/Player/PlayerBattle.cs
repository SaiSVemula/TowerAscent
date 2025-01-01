//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;

//public class PlayerBattle : MonoBehaviour
//{
//    [SerializeField] private int playerMaxHealth = 100;
//    private int playerCurrentHealth;

//    [SerializeField] private int playerBaseDefence = 0;
//    private int playerCurrentDefence;

//    [SerializeField] private List<Card> playerCardLoadout;

//    [SerializeField] private Slider playerHealthBar; // Player-specific health bar

//    // Temporary defense and healing effects stored as (value, timer) tuples
//    private List<(int value, int timer)> temporaryDefenses = new List<(int, int)>();
//    private List<(int value, int timer)> temporaryHeals = new List<(int, int)>();

//    public List<(int value, int timer)> TemporaryDefences => temporaryDefenses;
//    public List<(int value, int timer)> TemporaryHeals => temporaryHeals;

//    public bool HasActiveDefense => temporaryDefenses.Count > 0;
//    public bool HasActiveHealing => temporaryHeals.Count > 0;


//    public int PlayerCurrentHealth => playerCurrentHealth;
//    public int PlayerCurrentDefence => playerCurrentDefence;
//    public List<Card> PlayerCardLoadout => playerCardLoadout;

//    public Animator animator;

//    private void Awake()
//    {
//        playerCurrentHealth = playerMaxHealth;
//        playerCurrentDefence = playerBaseDefence;

//        playerHealthBar.maxValue = playerMaxHealth;
//        UpdatePlayerHealthBar();

//        playerCardLoadout = new List<Card>
//        {
//            Resources.Load<Card>("Cards/Axe Chop"),
//            Resources.Load<Card>("Cards/Fireball"),
//            Resources.Load<Card>("Cards/Dodge"),
//            Resources.Load<Card>("Cards/First Aid")
//        };

//        animator.SetBool("InBattle", true);

//    }

//    private void UpdatePlayerHealthBar()
//    {
//        if (playerHealthBar != null)
//        {
//            playerHealthBar.value = playerCurrentHealth;
//        }
//    }

//    public void PlayerTakeDamage(int damageAmount)
//    {
//        int netDamage = Mathf.Max(damageAmount - playerCurrentDefence, 0);
//        playerCurrentHealth = Mathf.Max(playerCurrentHealth - netDamage, 0);
//        UpdatePlayerHealthBar();
//        Debug.Log($"Player takes {netDamage} damage. Current health: {playerCurrentHealth}");
//        animator.SetTrigger("GetHit");
//    }

//    public void PlayerAddDefence(int defenceAmount)
//    {
//        playerCurrentDefence += defenceAmount;
//        Debug.Log($"Player defence increased by {defenceAmount}. Current defence: {playerCurrentDefence}");
//    }

//    public void PlayerHeal(int healAmount)
//    {
//        playerCurrentHealth = Mathf.Min(playerCurrentHealth + healAmount, playerMaxHealth);
//        Debug.Log($"Player heals {healAmount}. Current health: {playerCurrentHealth}");
//    }

//    public void PlayerResetDefence()
//    {
//        playerCurrentDefence = playerBaseDefence;
//        Debug.Log("Player defence reset to base.");
//    }

//    public void UsePlayerCard(int cardIndex, EnemyBattle targetEnemy)
//    {
//        if (cardIndex < 0 || cardIndex >= playerCardLoadout.Count) return;

//        Card selectedCard = playerCardLoadout[cardIndex];
//        if (selectedCard != null)
//        {
//            selectedCard.Use(this, targetEnemy);
//            animator.SetTrigger("Attack");
//        }

//    }

//    public void AddTemporaryDefence(int value, int timer)
//    {
//        Debug.Log($"Adding defense: Value = {value}, Timer = {timer}");
//        temporaryDefenses.Add((value, timer));
//        UpdateCurrentDefence();

//    }

//    public void AddTemporaryHealing(int value, int timer)
//    {
//        Debug.Log($"Adding healing: Value = {value}, Timer = {timer}");
//        temporaryHeals.Add((value, timer));
//    }

//    // Update current defense based on active temporary defenses
//    private void UpdateCurrentDefence()
//    {
//        playerCurrentDefence = playerBaseDefence + temporaryDefenses.Sum(d => d.value);
//        Debug.Log($"Player defense updated. Current defense: {playerCurrentDefence}");
//    }

//    // Decrement all effect timers and remove expired effects
//    public void DecrementEffectTimers()
//    {
//        // Defense timers
//        for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
//        {
//            temporaryDefenses[i] = (temporaryDefenses[i].value, temporaryDefenses[i].timer - 1);
//            if (temporaryDefenses[i].timer <= 0)
//            {
//                temporaryDefenses.RemoveAt(i);
//            }
//        }
//        UpdateCurrentDefence();

//        // Healing timers
//        for (int i = temporaryHeals.Count - 1; i >= 0; i--)
//        {
//            // Apply healing before decrementing timer
//            PlayerHeal(temporaryHeals[i].value);
//            temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
//            if (temporaryHeals[i].timer <= 0)
//            {
//                temporaryHeals.RemoveAt(i);
//            }
//        }
//    }
//}


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
        cardLoadout = new List<Card>
        {
        Resources.Load<Card>("Cards/Axe Chop"),
        Resources.Load<Card>("Cards/Fireball"),
        Resources.Load<Card>("Cards/Dodge"),
        Resources.Load<Card>("Cards/First Aid")
        };

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