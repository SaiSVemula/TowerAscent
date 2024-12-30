using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    public override void Initialize(EnemyDifficulty difficulty)
    {
        switch (difficulty)
        {
            case EnemyDifficulty.Hard:
                enemyMaxHealth = 200;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Hammer Smash"),
                    Resources.Load<Card>("Cards/Magic Block"),
                    Resources.Load<Card>("Cards/Healing Potion")
                };
                break;
            case EnemyDifficulty.Medium:
                enemyMaxHealth = 150;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Reflect")
                };
                break;
            case EnemyDifficulty.Easy:
                enemyMaxHealth = 100;
                enemyCardLoadout = new List<Card> { Resources.Load<Card>("Cards/Spear Thrust") };
                break;
        }
        enemyCurrentHealth = enemyMaxHealth;
    }
}
