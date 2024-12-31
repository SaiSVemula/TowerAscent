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

public class EnemyBattle : BattleEntity
{
    private BattleUI battleUI;
    public string EnemyName { get; private set; }
    private Difficulty difficulty;
    private EnemyType enemyType;

    public void Initialize(Difficulty gameDifficulty, EnemyType enemyType)
    {
        battleUI = FindObjectOfType<BattleUI>();
        difficulty = gameDifficulty;

        // Ensure card loadout is cleared and correctly initialized
        cardLoadout.Clear();
        SetupEnemyStatsAndCards();

        currentHealth = maxHealth;
        Debug.Log($"{EnemyName} initialized with {currentHealth} HP and {cardLoadout.Count} cards.");
    }


    private void SetupEnemyStatsAndCards()
    {
        switch (enemyType)
        {
            case EnemyType.Enemy1:
                ConfigureEnemy1();
                break;
            case EnemyType.Enemy2:
                ConfigureEnemy2();
                break;
            case EnemyType.Enemy3:
                ConfigureEnemy3();
                break;
            default:
                Debug.LogError("Invalid enemy type!");
                break;
        }
    }

    private void ConfigureEnemy1()
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                EnemyName = "Warden of Ash";
                maxHealth = 100;
                cardLoadout = new List<Card>
            {
                LoadCard("Cards/Spear Thrust"),
                LoadCard("Cards/Dodge"),
                LoadCard("Cards/First Aid")
            };
                break;
            case Difficulty.Medium:
                EnemyName = "Warden of Cinders";
                maxHealth = 70;
                cardLoadout = new List<Card>
            {
                LoadCard("Cards/Axe Chop")
            };
                break;
            case Difficulty.Easy:
                EnemyName = "Warden of Infernos";
                maxHealth = 50;
                cardLoadout = new List<Card>
            {
                LoadCard("Cards/Dagger Slash")
            };
                break;
        }
        Debug.Log($"{EnemyName} configured with {cardLoadout.Count} cards.");
    }


    private void ConfigureEnemy2()
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

    private void ConfigureEnemy3()
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

    private Card LoadCard(string path)
    {
        Card card = Resources.Load<Card>(path);
        if (card == null)
        {
            Debug.LogError($"Failed to load card at path: {path}");
        }
        return card;
    }


    public void AttackPlayer(PlayerBattle player)
    {
        if (cardLoadout.Count == 0)
        {
            Debug.LogWarning($"{EnemyName} has no cards to attack with!");
            return;
        }
        //Debug.Log($"{EnemyName} has {cardLoadout[0].name} cards to attack with.");

        int cardIndex = Random.Range(0, cardLoadout.Count);
        Card selectedCard = cardLoadout[cardIndex];
        if (selectedCard != null)
        {
            Debug.Log($"{EnemyName} uses {selectedCard.Name} against the player!");
            string logMessage = selectedCard.Use(this, player);
            //Debug.Log(logMessage);
            battleUI.AddBattleLog(logMessage);
        }
        else
        {
            Debug.LogError($"{EnemyName} tried to use an invalid card.");
        }
    }
}

public enum EnemyType
{
    Enemy1,
    Enemy2,
    Enemy3
}
