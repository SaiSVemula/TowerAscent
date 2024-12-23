using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackDamage { get; set; }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {Health}");
    }

    public void Attack(Player player)
    {
        Debug.Log("Enemy attacks the player.");
        player.TakeDamage(AttackDamage);
    }
}
