// Enemy.cs
using UnityEngine;

// This script handles the base out-of-battle behavior for enemies and allows for the creation
// of bespoke enemy objects with varying stats, behaviors, and metadata.
[CreateAssetMenu(menuName = "Enemy/BaseEnemy")]
public class Enemy : ScriptableObject
{
    // Enemy's basic information
    public string Name;
    public int MaxHealth;
    public int CurrentHealth;
    public int AttackDamage;

    // Meta information
    public int DifficultyLevel; 

    //Constructor
    public void Initialize(string name, int maxHealth, int attackDamage, int difficultyLevel)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        AttackDamage = attackDamage;
        DifficultyLevel = difficultyLevel;

        Debug.Log($"Enemy {Name} initialized with MaxHealth: {MaxHealth}, AttackDamage: {AttackDamage}, DifficultyLevel: {DifficultyLevel}");
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        Debug.Log($"{Name} took {damage} damage. Remaining health: {CurrentHealth}");
    }

    public void Attack(Player player)
    {
        Debug.Log($"{Name} attacks the player for {AttackDamage} damage.");
        player.TakeDamage(AttackDamage);
    }

    // Placeholder for additional logic
    // public void SpecialAbility() {}
    // public void Flee() {}
}
