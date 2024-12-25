using UnityEngine;

[CreateAssetMenu(menuName = "Card/MagicCard")]
public class MagicCard : Card
{
    [SerializeField] private int damage;

    // Serialized to allow effects like "Sleep" or "Reduce Attack"
    [SerializeField] private string effect; // not implemented yet

    public int Damage => damage;
    public string Effect => effect;// not implemented yet

    public override void Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        //main logic for magic card
        enemyBattle.TakeDamage(Damage);


        Debug.Log($"{Name} is used! It deals {Damage} magic damage to the enemy with effect: {Effect}");
        
        //future implementation of effects
        if (Effect == "Sleep")
        {
            Debug.Log($"{enemyBattle.EnemyName} is put to sleep and skips their next turn.");
            // Add sleep logic here
        }
        else if (Effect == "Reduce Attack")
        {
            Debug.Log($"{enemyBattle.EnemyName}'s attack power is reduced.");
            // Add attack reduction logic here
        }
    }
}
