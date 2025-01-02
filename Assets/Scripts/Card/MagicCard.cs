using UnityEngine;

[CreateAssetMenu(menuName = "Card/MagicCard")]
public class MagicCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        target.TakeDamage(damage);
        return $"{user.name} cast {Name}, dealing {damage} magic damage to {target.name}!";
    }
}