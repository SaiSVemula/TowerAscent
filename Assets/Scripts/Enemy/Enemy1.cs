using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy1")]
public class Enemy1 : EnemyBattle
{
    public override void Initialize(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                enemyMaxHealth = 100;
                enemyCardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Dodge"),
                    Resources.Load<Card>("Cards/First Aid")
                };
                break;
            case Difficulty.Medium:
                enemyMaxHealth = 70;
                enemyCardLoadout = new List<Card> { Resources.Load<Card>("Cards/Axe Chop") };
                break;
            case Difficulty.Easy:
                enemyMaxHealth = 50;
                enemyCardLoadout = new List<Card> { Resources.Load<Card>("Cards/Dagger Slash") };
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
