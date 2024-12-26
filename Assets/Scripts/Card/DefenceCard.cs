using UnityEngine;

// Defense card class works like a temporary armmor for the player
// best to play when you have low health and companion can attack for you
[CreateAssetMenu(menuName = "Card/DefenceCard")]
public class DefenceCard : Card
{
    [SerializeField] private int defence;

    public int Defence => defence;

    public override void Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        Debug.Log($"{Name} is used! It adds {Defence} defence to the player.");
        playerBattle.PlayerAddDefence(Defence);
    }
}
