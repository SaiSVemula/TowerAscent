using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy2")]
public class Enemy2 : EnemyBattle
{
    public override void Initialize(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                enemyMaxHealth = 200;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Hammer Smash"),
                    Resources.Load<Card>("Cards/Magic Block"),
                    Resources.Load<Card>("Cards/Healing Potion")
                };
                break;
            case Difficulty.Medium:
                enemyMaxHealth = 150;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Reflect")
                };
                break;
            case Difficulty.Easy:
                enemyMaxHealth = 100;
                enemyCardLoadout = new List<Card> { Resources.Load<Card>("Cards/Spear Thrust") };
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
