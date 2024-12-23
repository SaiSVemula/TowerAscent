using UnityEngine;

[CreateAssetMenu(menuName = "Card/HealingCard")]
public class HealingCard : Card
{
    [SerializeField] private int healingAmount;
    [SerializeField] private bool canRevive;

    public int HealingAmount => healingAmount;
    public bool CanRevive => canRevive;

    public override void Use(Player player, Enemy enemy)
    {
        if (CanRevive)
        {
            Debug.Log($"{Name} is used! Reviving a fallen ally with minimal health.");
            // Handle revive logic here
        }
        else
        {
            Debug.Log($"{Name} is used! Healing {HealingAmount} health.");
            player.Heal(HealingAmount);
        }
    }
}
