using UnityEngine;

[CreateAssetMenu(menuName = "Card/HealingCard")]
public class HealingCard : Card
{
    [SerializeField] private int healing;
    [SerializeField] private bool canRevive;//not added in yet 

    public int Healing => healing;
    public bool CanRevive => canRevive;

    public override void Use(PlayerBattle playerBattle, EnemyBattle enemyBattle)
    {
        if (CanRevive)
        {
            Debug.Log($"{Name} is used! Reviving a fallen ally with minimal health.");
            // Handle revive logic here
        }
        else
        {
            Debug.Log($"{Name} is used! Healing {Healing} health.");
            playerBattle.Heal(Healing);
        }
    }
}
