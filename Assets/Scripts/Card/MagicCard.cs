using UnityEngine;

[CreateAssetMenu(menuName = "Card/MagicCard")]
public class MagicCard : Card
{
    [SerializeField] private int damage;

    // Serialized to allow effects like "Sleep" or "Reduce Attack"
    [SerializeField] private string effect; // not implemented yet

    public int Damage => damage;
    public string Effect => effect;// not implemented yet

    public override string Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        enemyBattle.EnemyTakeDamage(Damage);
        string effectText = !string.IsNullOrEmpty(Effect) ? $" with {Effect} effect" : "";
        return $"Player cast {Name} dealing {Damage} magic damage{effectText}!";
    }
}
