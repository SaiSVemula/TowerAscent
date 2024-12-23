using System;
using System.Collections.Generic;

public class Player
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Defense { get; set; }
    public List<Card> Hand { get; set; }

    public Player(string name, int health, int Defence)//pass or add cards loadout here?
    {
        this.Name = name;
        this.Health = health;
        this.Defense = Defence;
        Hand = new List<Card>();
    }

    // Takes damage, applying the current defense value first.
    public void TakeDamage(int damage)
    {
        if (Defense > 0)
        {
            int mitigatedDamage = Math.Min(Defense, damage); // Reduce damage by defense amount
            Defense -= mitigatedDamage;
            damage -= mitigatedDamage;
            Console.WriteLine($"{Name}'s defense absorbed {mitigatedDamage} damage. Remaining defense: {Defense}");
        }

        if (damage > 0)
        {
            Health -= damage;
            Console.WriteLine($"{Name} took {damage} damage. Remaining health: {Health}");
        }
    }

    // Adds defense points to the player's current defense value.
    public void AddDefense(int amount)
    {
        Defense += amount;
        Console.WriteLine($"{Name} gained {amount} defense. Total defense: {Defense}");
    }

    // Restores health to the player up to their maximum health.
    public void Heal(int amount)
    {
        int maxHealth = 100;
        int healedAmount = Math.Min(amount, maxHealth - Health); // Prevent overhealing
        Health += healedAmount;
        Console.WriteLine($"{Name} healed {healedAmount} health. Current health: {Health}");
    }

    // Logs the player's current status for debugging or gameplay feedback.
    public void PrintStatus()
    {
        Console.WriteLine($"{Name}'s Status: Health = {Health}, Defense = {Defense}");
    }
}
