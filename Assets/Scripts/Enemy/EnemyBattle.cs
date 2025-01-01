//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public abstract class EnemyBattle : BattleEntity
//{
//    [SerializeField] private List<Card> cardLoadoutForDifficulty = new List<Card>();
//    public string EnemyName { get; protected set; }

//    protected Difficulty difficulty;

//    public void Initialize(Difficulty gameDifficulty)
//    {
//        difficulty = gameDifficulty;
//        SetupEnemyStatsAndCards();
//        currentHealth = maxHealth; 
//    }

//    protected abstract void SetupEnemyStatsAndCards();

//    public void AttackPlayer(PlayerBattle player)
//    {
//        if (cardLoadout.Count == 0)
//        {
//            Debug.LogWarning($"{EnemyName} has no cards to attack with!");
//            return;
//        }

//        // Randomly select a card for the enemy to use
//        int cardIndex = Random.Range(0, cardLoadout.Count);
//        Card selectedCard = cardLoadout[cardIndex];
//        //Debug.Log($"Selected card: {selectedCard.Name}");

//        if (selectedCard != null)
//        {
//            Debug.Log($"{EnemyName} uses {selectedCard.Name} against the player!");
//            string logMessage = selectedCard.Use(this, player);
//            Debug.Log(logMessage);
//        }
//        else
//        {
//            Debug.LogError($"{EnemyName} tried to use an invalid card.");
//        }
//    }


//    public override void UseCard(int cardIndex, BattleEntity target)
//    {
//        if (cardIndex < 0 || cardIndex >= cardLoadout.Count)
//        {
//            return;
//        }

//        Card selectedCard = cardLoadout[cardIndex];
//        if (selectedCard != null)
//        {
//            Debug.Log($"{EnemyName} used {selectedCard.Name} on {target.name}");
//            animator.SetTrigger("Attack");
//            Debug.Log(selectedCard.Use(this, target));
//        }
//    }
//}


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
    private Difficulty difficulty;
    private EnemyType enemyType;

    public EnemyBattle Initialize(Difficulty gameDifficulty, EnemyType enemyType)
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
            case Difficulty.Hard:
                EnemyName = "Warden of Ash";
                maxHealth = 100;
                cardLoadout = new List<Card>
                {
                Resources.Load<Card>("Cards/Spear Thrust"),
                Resources.Load<Card>("Cards/Dodge"),
                Resources.Load<Card>("Cards/First Aid")
                };
                break;
            case Difficulty.Medium:
                EnemyName = "Warden of Cinders";
                maxHealth = 70;
                cardLoadout = new List<Card>
                {
                Resources.Load<Card>("Cards/Axe Chop")
                };
                break;
            case Difficulty.Easy:
                EnemyName = "Warden of Infernos";
                maxHealth = 50;
                cardLoadout = new List<Card>
                {
                Resources.Load<Card>("Cards/Dagger Slash")
                };
                break;
        }
        Debug.Log($"{EnemyName} configured with {cardLoadout.Count} cards.");
    }

    private void ConfigureEnemyType2()
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                EnemyName = "Dusk Herald";
                maxHealth = 200;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Hammer Smash"),
                    Resources.Load<Card>("Cards/Magic Block"),
                    Resources.Load<Card>("Cards/Healing Potion")
                };
                break;
            case Difficulty.Medium:
                EnemyName = "Twilight Regent";
                maxHealth = 150;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Reflect")
                };
                break;
            case Difficulty.Easy:
                EnemyName = "Midnight Emperor";
                maxHealth = 100;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust")
                };
                break;
        }
    }

    private void ConfigureEnemyType3()
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                EnemyName = "The Eternal Verdict";
                maxHealth = 300;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Meteor Shower")
                };
                break;
            case Difficulty.Medium:
                EnemyName = "The Shadowed Adjudicator";
                maxHealth = 250;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
            case Difficulty.Easy:
                EnemyName = "The Silent Judge";
                maxHealth = 150;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
        }
    }

    //public void AttackPlayer(PlayerBattle player, EnemyBattle enemy)
    //{
    //    if (cardLoadout.Count == 0)//should not happen as each enemy instance loads with atleast one card.
    //    {
    //        Debug.LogWarning($"{EnemyName} has no cards to attack with!");
    //        return;
    //    }

    //    // Enemy plays the cards based on a random number.
    //    int cardIndex = Random.Range(0, cardLoadout.Count);
    //    Card selectedCard = cardLoadout[cardIndex];
    //    if (selectedCard != null)
    //    {
    //        if (selectedCard == null || player == null || enemy == null)
    //        {
    //            Debug.LogError("Either selectedCard or player is null in EnemyBattle.AttackPlayer.");
    //            return;
    //        }

    //        Debug.Log($"{EnemyName} uses {selectedCard.Name} against {player.name}!");
    //        string log = selectedCard.Use(enemy, player);
    //        battleUI.AddBattleLog(log);
    //    }
    //    else
    //    {
    //        Debug.LogError($"{EnemyName} tried to use an invalid card.");
    //    }
    //    UpdateHealthBar();
    //}

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