using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    WeaponAttack,
    MagicAttack,
    Defense,
    Healing
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public string description;

    // Fields for specific card types
    public int damage;
    public int defenseLevel;
    public int healingAmount;

    // Fields for special card types
    public bool canStun;
}

