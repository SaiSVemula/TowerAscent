using UnityEngine;

[CreateAssetMenu(menuName = "Card/DefenseCard")]
public class DefenseCard : Card
{
    [SerializeField] private int defenseValue;

    public int DefenseValue => defenseValue;

    public override void Use(Player player, Enemy enemy)
    {
        Debug.Log($"{Name} is used! It adds {DefenseValue} defense to the player.");
        player.AddDefense(DefenseValue);
    }
}
