using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CombinationCard")]
public class CombinationCard : Card
{
    [SerializeField] private List<Card> subCards;

    public List<Card> SubCards => subCards;

    public void Initialize(string name, string description, CardType type, List<Card> subCards)
    {
        this.name = name;
        this.description = description;
        this.type = type;
        this.subCards = subCards;
    }

    public override string Use(BattleEntity user, BattleEntity target)
    {
        string log = $"{user.name} used {Name}, activating combination effects:\n";

        foreach (var subCard in subCards)
        {
            log += subCard.Use(user, target) + "\n";
        }

        return log.TrimEnd();
    }
}
