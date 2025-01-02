using UnityEngine;

// Defense card class works like a temporary armmor for the player
// best to play when you have low health and companion can attack for you

[CreateAssetMenu(menuName = "Card/DefenceCard")]
public class DefenceCard : Card
{
    [SerializeField] private int defence;
    [SerializeField] private int timer;

    public int Defence => defence;
    public int Timer => timer;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        user.AddTemporaryDefence(Defence, Timer);
        return $"{user.name} used {Name}, gaining {Defence} defense for {Timer} turns!";
    }
}
