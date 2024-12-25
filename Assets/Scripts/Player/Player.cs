// Player.cs
using UnityEngine;

// This script handles the out-of-battle behavior for the player, including inventory,
// player stats, card preset loadouts, and other meta information.
public class Player : MonoBehaviour
{
    // Player's basic information
    public string Name;
    public int Wins;
    public int Losses;

    // Meta information
    public int NpcInteractions;
    public int QuestsCompleted;

    // Inventory system (not implemented yet)
    // public List<Item> Inventory;

    // Card preset loadouts (for quick setup in battles, not in-battle management)
    // public List<Card> CardPresetLoadouts;

    // Player's health-related stats
    public int MaxHealth = 100;
    public int CurrentHealth;

    // Example initialization of the player
    public void Initialize(string name, int maxHealth)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Wins = 0;
        Losses = 0;
        NpcInteractions = 0;
        QuestsCompleted = 0;

        Debug.Log($"Player {Name} initialized with MaxHealth: {MaxHealth}");
    }

    public void RecordWin()
    {
        Wins++;
        Debug.Log($"{Name} recorded a win! Total wins: {Wins}");
    }

    public void RecordLoss()
    {
        Losses++;
        Debug.Log($"{Name} recorded a loss! Total losses: {Losses}");
    }

    public void InteractWithNpc()
    {
        NpcInteractions++;
        Debug.Log($"{Name} interacted with an NPC. Total interactions: {NpcInteractions}");
    }

    public void CompleteQuest()
    {
        QuestsCompleted++;
        Debug.Log($"{Name} completed a quest. Total quests completed: {QuestsCompleted}");
    }

    public void Heal(int amount)
    {
        int healedAmount = Mathf.Min(amount, MaxHealth - CurrentHealth);
        CurrentHealth += healedAmount;
        Debug.Log($"{Name} healed {healedAmount} health. Current health: {CurrentHealth}");
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        Debug.Log($"{Name} took {damage} damage. Remaining health: {CurrentHealth}");
    }
}
