using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum CompanionType
{
    None = 0,
    Companion1,
    Companion2,
    Companion3,
}
public class CompanionBattle : BattleEntity
{
    public string CompanionName { get; private set; }
    [SerializeField] private Slider CompanionHealthBar;

    public void Initialize(CompanionType companionType)
    {
        switch (companionType)
        {
            case CompanionType.Companion1:
                CompanionName = "companion1";
                maxHealth = 50;
                baseDefence = 0;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Dagger Slash"),
                    Resources.Load<Card>("Cards/Healing Cards/First Aid")
                };
                break;

            case CompanionType.Companion2:
                CompanionName = "companion2";
                maxHealth = 100;
                baseDefence = 4;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Magic Cards/Fireball"),
                    Resources.Load<Card>("Cards/Defence Cards/Magic Barrier")
                };
                break;

            case CompanionType.Companion3:
                CompanionName = "companion3";
                maxHealth = 150;
                baseDefence = 7;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Hammer Smash"),
                    Resources.Load<Card>("Cards/Defence Cards/Dodge"),
                    Resources.Load<Card>("Cards/Healing Cards/First Aid")
                };
                break;

            default:
                Debug.LogError("Invalid companion type!");
                return;
        }

        // Update health and defense
        currentHealth = maxHealth;
        currentDefence = baseDefence;

        if (CompanionHealthBar != null)
        {
            healthBar = CompanionHealthBar;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        else
        {
            Debug.LogError("CompanionHealthBar is not assigned in the Inspector!");
        }

        Debug.Log($"{CompanionName} initialized with {maxHealth} HP and {baseDefence} defense.");
    }

    public void AttackEnemy(EnemyBattle targetEnemy, BattleUI battleUI)
    {
        if (targetEnemy == null)
        {
            Debug.LogError($"{CompanionName} has no target to attack.");
            return;
        }
    
        if (cardLoadout == null || cardLoadout.Count == 0)
        {
            Debug.LogWarning($"{CompanionName} has no cards to attack with.");
            return;
        }
    
        // Select a card randomly or based on logic
        int cardIndex = Random.Range(0, cardLoadout.Count);
        Card selectedCard = cardLoadout[cardIndex];
    
        if (selectedCard == null)
        {
            Debug.LogError("Selected card is null for companion attack.");
            return;
        }
    
        // Use the card's effect
        string logMessage = selectedCard.Use(this, targetEnemy);
    
        // Log the attack to the console
        Debug.Log($"{CompanionName} used {selectedCard.Name} on {targetEnemy.EnemyName}.");
        Debug.Log(logMessage);
    
        // Log the attack to the BattleUI feed
        if (battleUI != null)
        {
            battleUI.AddBattleLog(logMessage);
            //battleUI.AddBattleLog($"{CompanionName} used {selectedCard.Name} on {targetEnemy.EnemyName}. {logMessage}");
        }
    
        // Check if the enemy has been defeated
        if (targetEnemy.CurrentHealth <= 0)
        {
            Debug.Log($"{CompanionName} defeated {targetEnemy.EnemyName}!");
            if (battleUI != null)
            {
                battleUI.AddBattleLog($"{CompanionName} defeated {targetEnemy.EnemyName}!");
            }
        }
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        UpdateHealthBar(); // Update the health bar after taking damage
    }

    public override void Heal(int healAmount)
    {
        base.Heal(healAmount);
        UpdateHealthBar(); // Update the health bar after healing
    }

    protected override void Die()
    {
        Debug.Log($"{CompanionName} has been defeated!");
        gameObject.SetActive(false); // Disable the companion
    }
}