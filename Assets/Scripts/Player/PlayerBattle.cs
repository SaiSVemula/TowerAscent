using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private int playerMaxHealth = 100;
    private int playerCurrentHealth;

    [SerializeField] private int playerBaseDefence = 0;
    private int playerCurrentDefence;

    [SerializeField] private List<Card> playerCardLoadout;

    [SerializeField] private Slider playerHealthBar; // Player-specific health bar

    // Temporary defense and healing effects stored as (value, timer) tuples
    private List<(int value, int timer)> temporaryDefenses = new List<(int, int)>();
    private List<(int value, int timer)> temporaryHeals = new List<(int, int)>();

    public List<(int value, int timer)> TemporaryDefenses => temporaryDefenses;
    public List<(int value, int timer)> TemporaryHeals => temporaryHeals;


    public int PlayerCurrentHealth => playerCurrentHealth;
    public int PlayerCurrentDefence => playerCurrentDefence;
    public List<Card> PlayerCardLoadout => playerCardLoadout;

    private void Awake()
    {
        playerCurrentHealth = playerMaxHealth;
        playerCurrentDefence = playerBaseDefence;

        playerHealthBar.maxValue = playerMaxHealth;
        UpdatePlayerHealthBar();

        playerCardLoadout = new List<Card>
        {
            Resources.Load<Card>("Cards/Axe Chop"),
            Resources.Load<Card>("Cards/Fireball"),
            Resources.Load<Card>("Cards/Dodge"),
            Resources.Load<Card>("Cards/First Aid")
        };
    }

    public void UpdatePlayerHealthBar()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHealth;
        }
    }

    public void PlayerTakeDamage(int damageAmount)
    {
        int netDamage = Mathf.Max(damageAmount - playerCurrentDefence, 0);
        playerCurrentHealth = Mathf.Max(playerCurrentHealth - netDamage, 0);
        Debug.Log($"Player takes {netDamage} damage. Current health: {playerCurrentHealth}");
    }

    public void PlayerAddDefence(int defenceAmount)
    {
        playerCurrentDefence += defenceAmount;
        Debug.Log($"Player defence increased by {defenceAmount}. Current defence: {playerCurrentDefence}");
    }

    public void PlayerHeal(int healAmount)
    {
        playerCurrentHealth = Mathf.Min(playerCurrentHealth + healAmount, playerMaxHealth);
        Debug.Log($"Player heals {healAmount}. Current health: {playerCurrentHealth}");
    }

    public void PlayerResetDefence()
    {
        playerCurrentDefence = playerBaseDefence;
        Debug.Log("Player defence reset to base.");
    }

    public void UsePlayerCard(int cardIndex, EnemyBattle targetEnemy)
    {
        if (cardIndex < 0 || cardIndex >= playerCardLoadout.Count) return;

        Card selectedCard = playerCardLoadout[cardIndex];
        if (selectedCard != null)
        {
            selectedCard.Use(this, targetEnemy);
        }
    }

    public void AddTemporaryDefence(int value, int timer)
    {
        Debug.Log($"Adding defense: Value = {value}, Timer = {timer}");
        temporaryDefenses.Add((value, timer));
        UpdateCurrentDefence();
    }

    public void AddTemporaryHealing(int value, int timer)
    {
        Debug.Log($"Adding healing: Value = {value}, Timer = {timer}");
        temporaryHeals.Add((value, timer));
    }

    // Update current defense based on active temporary defenses
    private void UpdateCurrentDefence()
    {
        playerCurrentDefence = playerBaseDefence + temporaryDefenses.Sum(d => d.value);
        Debug.Log($"Player defense updated. Current defense: {playerCurrentDefence}");
    }

    // Decrement all effect timers and remove expired effects
    public void DecrementEffectTimers()
    {
        Debug.Log("Decrementing Timers");

        // Decrement defense timers
        for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
        {
            Debug.Log($"Before: Defense Value = {temporaryDefenses[i].value}, Timer = {temporaryDefenses[i].timer}");
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
            Debug.Log($"Before: Healing Value = {temporaryHeals[i].value}, Timer = {temporaryHeals[i].timer}");
            playerCurrentHealth = Mathf.Min(playerCurrentHealth + temporaryHeals[i].value, playerMaxHealth);
            temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
            if (temporaryHeals[i].timer <= 0)
            {
                temporaryHeals.RemoveAt(i);
            }
        }
        UpdatePlayerHealthBar();
    }
}