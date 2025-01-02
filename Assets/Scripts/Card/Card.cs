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
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private CardType type;
    [SerializeField] private Sprite cardSprite;

    public string Name => name;
    public string Description => description;
    public CardType Type => type;
    public Sprite CardSprite => cardSprite;

    // Generalized Use method for both player and enemy
    public abstract string Use(BattleEntity user, BattleEntity target);
}
