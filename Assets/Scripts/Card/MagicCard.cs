using UnityEngine;

[CreateAssetMenu(menuName = "Card/MagicCard")]
public class MagicCard : Card
{
    [SerializeField] private int magicDamage;
    [SerializeField] private string effect; // Serialized to allow effects like "Sleep" or "Reduce Attack"

    public int MagicDamage => magicDamage;
    public string Effect => effect;

    public override void Use(Player player, Enemy enemy)
    {
        Debug.Log($"{Name} is used! It deals {MagicDamage} magic damage to the enemy with effect: {Effect}");
        enemy.TakeDamage(MagicDamage);

        if (Effect == "Sleep")
        {
            Debug.Log($"{enemy.Name} is put to sleep and skips their next turn.");
            // Add sleep logic here
        }
        else if (Effect == "Reduce Attack")
        {
            Debug.Log($"{enemy.Name}'s attack power is reduced.");
            // Add attack reduction logic here
        }
    }
}
