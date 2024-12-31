//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//[CreateAssetMenu(menuName = "Enemy/Enemy3")]
//public class Enemy3 : EnemyBattle
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
//                EnemyName = "The Eternal Verdict";
//                maxHealth = 300;
//                cardLoadout = new List<Card>
//                {
//                    Resources.Load<Card>("Cards/Sword Slash"),
//                    Resources.Load<Card>("Cards/Group Heal"),
//                    Resources.Load<Card>("Cards/Magic Barrier"),
//                    Resources.Load<Card>("Cards/Meteor Shower")
//                };
//                // Companion logic can be added here.
//                break;
//            case Difficulty.Medium:
//                EnemyName = "The Shadowed Adjudicator";
//                maxHealth = 250;
//                cardLoadout = new List<Card>
//                {
//                    Resources.Load<Card>("Cards/Sword Slash"),
//                    Resources.Load<Card>("Cards/Group Heal"),
//                    Resources.Load<Card>("Cards/Magic Barrier"),
//                    Resources.Load<Card>("Cards/Earthquake")
//                };
//                break;
//            case Difficulty.Easy:
//                EnemyName = "The Silent Judge";
//                maxHealth = 150;
//                cardLoadout = new List<Card>
//                {
//                    Resources.Load<Card>("Cards/Sword Slash"),
//                    Resources.Load<Card>("Cards/Earthquake")
//                };
//                break;
//        }
//    }
//}
