using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    Enemy1,
    Enemy2,
    Enemy3
}

public class EnemyBattle : BattleEntity
{
    [SerializeField] private Slider EnemyHealthBar;
    private BattleUI battleUI;
    public string EnemyName { get; private set; }
    private int difficulty;
    private EnemyType enemyType;

    public EnemyBattle Initialize(int gameDifficulty, EnemyType enemyType)
    {
        Debug.Log($"Initializing Enemy with Difficulty: {gameDifficulty}, Type: {enemyType}");

        this.difficulty = gameDifficulty;
        this.enemyType = enemyType;

        if (EnemyHealthBar == null)
        {
            Debug.LogError("Enemy health bar is not assigned!");
            return null;
        }

        SetupEnemyStatsAndCards();

        if (maxHealth <= 0)
        {
            Debug.LogError($"maxHealth not set! Ensure ConfigureEnemyX methods are executed. Current Enemy: {EnemyName}");
        }

        healthBar = EnemyHealthBar;
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;

        Debug.Log($"Enemy initialized: Name={EnemyName}, MaxHealth={maxHealth}, Cards={cardLoadout.Count}");
        UpdateHealthBar();

        return this;
    }


    private void SetupEnemyStatsAndCards()
    {
        Debug.Log($"Configuring {enemyType} for difficulty: {difficulty}");
        switch (enemyType)
        {
            case EnemyType.Enemy1:
                ConfigureEnemyType1();
                break;
            case EnemyType.Enemy2:
                ConfigureEnemyType2();
                break;
            case EnemyType.Enemy3:
                ConfigureEnemyType3();
                break;
            default:
                Debug.LogError("Invalid enemy type!");
                return;
        }
        Debug.Log($"Configuration complete for {EnemyName}: Max Health: {maxHealth}, Cards: {cardLoadout.Count}");
    }


    private void ConfigureEnemyType1()
    {
        switch (difficulty)
        {
            case 2:
                EnemyName = "Warden of Ash";
                maxHealth = 100;
                cardLoadout = new List<Card>
                {
                Resources.Load<Card>("Cards/Weapon Cards/Spear Thrust"),
                Resources.Load<Card>("Cards/Defence Cards/Dodge"),
                Resources.Load<Card>("Cards/Healing Cards/First Aid")
                };
                break;
            case 1:
                EnemyName = "Warden of Cinders";
                maxHealth = 70;
                cardLoadout = new List<Card>
                {
                Resources.Load<Card>("Cards/Weapon Cards/Axe Chop")
                };
                break;
            case 0:
                EnemyName = "Warden of Infernos";
                maxHealth = 50;
                cardLoadout = new List<Card>
                {
                Resources.Load<Card>("Cards/Weapon Cards/Dagger Slash")
                };
                break;
        }
        Debug.Log($"{EnemyName} configured with {cardLoadout.Count} cards.");
    }

    private void ConfigureEnemyType2()
    {
        switch (difficulty)
        {
            case 2:
                EnemyName = "Dusk Herald";
                maxHealth = 200;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Hammer Smash"),
                    Resources.Load<Card>("Cards/Defence Cards/Magic Block"),
                    Resources.Load<Card>("Cards/Healing Cards/Healing Potion")
                };
                break;
            case 1:
                EnemyName = "Twilight Regent";
                maxHealth = 150;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Defence Cards/Reflect")
                };
                break;
            case 0:
                EnemyName = "Midnight Emperor";
                maxHealth = 100;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Spear Thrust")
                };
                break;
        }
    }

    private void ConfigureEnemyType3()
    {
        switch (difficulty)
        {
            case 2:
                EnemyName = "The Eternal Verdict";
                maxHealth = 300;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Magic Cards/Meteor Shower"),
                    Resources.Load<Card>("Cards/Defence Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Healing Cards/Group Heal")
                };
                break;
            case 1:
                EnemyName = "The Shadowed Adjudicator";
                maxHealth = 250;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Magic Cards/Earthquake"),
                    Resources.Load<Card>("Cards/Defence Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Healing Cards/Group Heal")
                };
                break;
            case 0:
                EnemyName = "The Silent Judge";
                maxHealth = 150;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Weapon Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Magic Cards/Earthquake")
                };
                break;
        }
    }

    public void AttackPlayer(PlayerBattle player)
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null in EnemyBattle.AttackPlayer!");
            return;
        }

        if (cardLoadout == null || cardLoadout.Count == 0)
        {
            Debug.LogError($"{EnemyName} has no cards in its card loadout!");
            return;
        }

        int cardIndex = Random.Range(0, cardLoadout.Count);
        Card selectedCard = cardLoadout[cardIndex];

        if (selectedCard == null)
        {
            Debug.LogError($"Selected card is null for {EnemyName}!");
            return;
        }

        Debug.Log($"{EnemyName} uses {selectedCard.Name} on {player.name}.");
        selectedCard.Use(this, player);
    }
}