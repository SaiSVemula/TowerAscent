using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/CompanionCard")]
public class CompanionCard : ScriptableObject
{
    [SerializeField] private string companionName; // Name of the companion
    [SerializeField] private Sprite companionSprite; // Sprite representing the companion
    [SerializeField] private string description; // Companion description
    [SerializeField] private int health; // Companion's health
    [SerializeField] private int defense; // Companion's defense
    [SerializeField] private CompanionType type;

    public string CompanionName => companionName;
    public Sprite CompanionSprite => companionSprite;
    public CompanionType Type => type;
    public string Description => description;
    public int Health => health;
    public int Defense => defense;
}
