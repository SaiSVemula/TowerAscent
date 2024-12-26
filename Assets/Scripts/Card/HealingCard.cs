using UnityEngine;

[CreateAssetMenu(menuName = "Card/HealingCard")]
public class HealingCard : Card
{
    [SerializeField] private int healing;
    [SerializeField] private int timer; // How many turns the healing lasts
    [SerializeField] private bool canRevive;//not added in yet 

    public int Healing => healing;
    public bool CanRevive => canRevive;
    public int Timer => timer;

    public override void Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        if (CanRevive)
        {
            Debug.Log($"{Name} is used! Reviving a fallen ally with minimal health.");
            // Handle revive logic here
        }
        else
        {
            Debug.Log($"{Name} is used! It heals {Healing} health for {Timer} turns.");
            playerBattle.AddTemporaryHealing(Healing, Timer); // Pass card-specific timer
        }
    }

}
