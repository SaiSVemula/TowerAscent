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

    public override string Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        if (CanRevive)
        {
            return $"Player used {Name} attempting to revive!";
        }

        playerBattle.AddTemporaryHealing(Healing, Timer);
        return $"Player used {Name} healing {Healing} HP for {Timer} turns!";
    }
}
