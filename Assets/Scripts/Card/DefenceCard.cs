using UnityEngine;

// Defense card class works like a temporary armmor for the player
// best to play when you have low health and companion can attack for you
[CreateAssetMenu(menuName = "Card/DefenceCard")]
public class DefenceCard : Card
{
    [SerializeField] private int defence; // How much defence the card adds
    [SerializeField] private int timer; // How many turns the defence lasts

    public int Defence => defence;
    public int Timer => timer;

    public override void Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        Debug.Log($"{Name} is used! It adds {Defence} defense for {Timer} turns.");
        playerBattle.AddTemporaryDefence(Defence, Timer); // Pass card-specific timer
    }

}
