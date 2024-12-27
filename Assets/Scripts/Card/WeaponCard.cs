using UnityEngine;

[CreateAssetMenu(menuName = "Card/WeaponCard")]
public class WeaponCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override string Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        enemyBattle.EnemyTakeDamage(Damage);
        return $"Player used {Name} dealing {Damage} physical damage!";
    }
}
