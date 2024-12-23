using UnityEngine;

[CreateAssetMenu(menuName = "Card/WeaponCard")]
public class WeaponCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override void Use(Player player, Enemy enemy)
    {
        Debug.Log($"{Name} is used! It deals {Damage} physical damage to the enemy.");
        enemy.TakeDamage(Damage);
    }
}
