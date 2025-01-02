//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//[CreateAssetMenu(menuName = "Enemy/Enemy1")]
//public class Enemy1 : EnemyBattle
//{
//    [SerializeField] private Slider healthBar;
//    protected override void SetupEnemyStatsAndCards()
//    {
//        //animator = GetComponent<Animator>();
//        //animator.SetBool("InBattle", true);
//        switch (difficulty)
//        {
//            case Difficulty.Hard:
//                EnemyName = "Warden of Ash";
//                maxHealth = 100;
//                cardLoadout = new List<Card>
//                {
//                    Resources.Load<Card>("Cards/Spear Thrust"),
//                    Resources.Load<Card>("Cards/Dodge"),
//                    Resources.Load<Card>("Cards/First Aid")
//                };
//                break;
//            case Difficulty.Medium:
//                EnemyName = "Warden of Cinders";
//                maxHealth = 70;
//                cardLoadout = new List<Card> { Resources.Load<Card>("Cards/Axe Chop") };
//                break;
//            case Difficulty.Easy:
//                EnemyName = "Warden of Infernos";
//                maxHealth = 50;
//                cardLoadout = new List<Card> { Resources.Load<Card>("Cards/Dagger Slash") };
//                break;
//        }
//    }
//}
