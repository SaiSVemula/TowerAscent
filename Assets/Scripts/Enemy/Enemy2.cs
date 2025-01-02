//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//[CreateAssetMenu(menuName = "Enemy/Enemy2")]
//public class Enemy2 : EnemyBattle
//{
//    [SerializeField] private Slider healthBar;
//    protected override void SetupEnemyStatsAndCards()
//    {
//        healthBar = GetComponent<Slider>();
//        animator = GetComponent<Animator>();
//        //animator.SetBool("InBattle", true);
//        switch (difficulty)
//        {
//            case Difficulty.Hard:
//                EnemyName = "Dusk Herald";
//                maxHealth = 200;
//                cardLoadout = new List<Card>
//                {
//                    Resources.Load<Card>("Cards/Hammer Smash"),
//                    Resources.Load<Card>("Cards/Magic Block"),
//                    Resources.Load<Card>("Cards/Healing Potion")
//                };
//                break;
//            case Difficulty.Medium:
//                EnemyName = "Twilight Regent";
//                maxHealth = 150;
//                cardLoadout = new List<Card>
//                {
//                    Resources.Load<Card>("Cards/Spear Thrust"),
//                    Resources.Load<Card>("Cards/Reflect")
//                };
//                break;
//            case Difficulty.Easy:
//                EnemyName = "Midnight Emperor";
//                maxHealth = 100;
//                cardLoadout = new List<Card> { Resources.Load<Card>("Cards/Spear Thrust") };
//                break;
//        }
//    }
//}
