using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : Enemy
{
    public override void Initialize(EnemyDifficulty difficulty)
    {
        switch (difficulty)
        {
            case EnemyDifficulty.Hard:
                enemyMaxHealth = 300;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Meteor Shower") 
                };
                // Companion logic can be added here.
                break;
            case EnemyDifficulty.Medium:
                enemyMaxHealth = 250;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
            case EnemyDifficulty.Easy:
                enemyMaxHealth = 150;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
        }
        enemyCurrentHealth = enemyMaxHealth;
    }
}
