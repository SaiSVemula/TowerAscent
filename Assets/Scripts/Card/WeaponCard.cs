using UnityEngine;

[CreateAssetMenu(menuName = "Card/WeaponCard")]
public class WeaponCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override void Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)//player not really used
    {
        Debug.Log($"{Name} is used! It deals {Damage} physical damage to the enemy.");
        enemyBattle.TakeDamage(Damage);
    }
}
