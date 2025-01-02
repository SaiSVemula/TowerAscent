using UnityEngine;

[CreateAssetMenu(menuName = "Card/HealingCard")]
public class HealingCard : Card
{
    [SerializeField] private int healing;
    [SerializeField] private int timer;
    [SerializeField] private bool canRevive;//not added in yet 
    public int Healing => healing;
    public int Timer => timer;
    public bool CanRevive => canRevive;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        if (CanRevive)
        {
            return $"{user.name} used {Name} attempting to revive!";
        }

        user.AddTemporaryHealing(Healing, Timer);
        return $"{user.name} used {Name}, healing {Healing} HP for {Timer} turns!";
    }
}
