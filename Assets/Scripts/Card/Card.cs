using UnityEngine;

public enum CardType
{
    Common,
    Rare,
    Epic,
    Legendary
}

//abstract class for all cards to inherit 

public abstract class Card : ScriptableObject
{
    [SerializeField] protected string name;
    [SerializeField] protected string description;
    [SerializeField] protected CardType type;
    [SerializeField] protected Sprite cardSprite;

    public string Name => name;
    public string Description => description;
    public CardType Type => type;
    public Sprite CardSprite => cardSprite;

    // Generalized Use method for both player and enemy
    public abstract string Use(BattleEntity user, BattleEntity target);
}
