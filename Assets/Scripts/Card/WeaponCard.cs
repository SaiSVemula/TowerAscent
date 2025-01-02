using UnityEngine;

[CreateAssetMenu(menuName = "Card/WeaponCard")]
public class WeaponCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        target.TakeDamage(damage);
        return $"{user.name} used {Name}, dealing {damage} damage to {target.name}!";
    }
}
