using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy3")]
public class Enemy3 : EnemyBattle
{
    public override void Initialize(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
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
            case Difficulty.Medium:
                enemyMaxHealth = 250;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
            case Difficulty.Easy:
                enemyMaxHealth = 150;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
        }
        enemyCurrentHealth = enemyMaxHealth;
        UpdateHealthBar();
        LogEnemyInfo();
    }

    public override void AttackPlayer(PlayerBattle player)
    {
        Debug.Log($"{enemyName} attacks the player!");
        // Implement specific attack logic here
    }

}
