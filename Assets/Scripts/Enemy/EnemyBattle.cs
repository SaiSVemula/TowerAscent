using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBattle : BattleEntity
{
    [SerializeField] private List<Card> cardLoadoutForDifficulty = new List<Card>();
    public string EnemyName { get; protected set; }

    protected Difficulty difficulty;

    public void Initialize(Difficulty gameDifficulty)
    {
        difficulty = gameDifficulty;
        SetupEnemyStatsAndCards();
        currentHealth = maxHealth; // Ensure health is set correctly
    }

    protected abstract void SetupEnemyStatsAndCards();

    public override void UseCard(int cardIndex, BattleEntity target)
    {
        if (cardIndex < 0 || cardIndex >= cardLoadout.Count)
        {
            return;
        }

        Card selectedCard = cardLoadout[cardIndex];
        if (selectedCard != null)
        {
            Debug.Log($"{EnemyName} used {selectedCard.Name} on {target.name}");
            //animator.SetTrigger("Attack");
            Debug.Log(selectedCard.Use(this, target));
        }
    }
}
