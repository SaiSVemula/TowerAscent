using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : Enemy
{
    public override void Initialize(EnemyDifficulty difficulty)
    {
        switch (difficulty)
        {
            case EnemyDifficulty.Hard:
                enemyMaxHealth = 100;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Dodge"),
                    Resources.Load<Card>("Cards/First Aid")
                };
                break;
            case EnemyDifficulty.Medium:
                enemyMaxHealth = 70;
                enemyCardLoadout = new List<Card> { Resources.Load<Card>("Cards/Axe Chop") };
                break;
            case EnemyDifficulty.Easy:
                enemyMaxHealth = 50;
                enemyCardLoadout = new List<Card> { Resources.Load<Card>("Cards/Dagger Slash") };
                break;
        }
        enemyCurrentHealth = enemyMaxHealth;
    }
}
