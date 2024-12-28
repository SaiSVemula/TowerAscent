using UnityEngine;

//abstract class for all cards to inherit 
public abstract class Card : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private string status;
    [SerializeField] private Sprite cardSprite;

    public string Name => name;
    public string Description => description;
    public string Status => status;
    public Sprite CardSprite => cardSprite;

    public void Initialize(string Name, string Description, string Status)
    {
        name = Name;
        description = Description;
        status = Status;
    }

    public abstract string Use(PlayerBattle player, EnemyBattle enemy);
}
