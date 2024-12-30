//// CardGenerator.cs
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

////code made with Copilot AI to quickly make the cards.
//public static class CardGenerator
//{
//    private const string CardFolderPath = "Assets/Cards";

//    [MenuItem("Tools/Generate Cards")]
//    public static void GenerateCards()
//    {
//        if (!AssetDatabase.IsValidFolder(CardFolderPath))
//        {
//            AssetDatabase.CreateFolder("Assets", "Cards");
//        }

//        // Generate Weapon Cards
//        CreateWeaponCard("Axe Chop", "Deals moderate physical damage to a single enemy.", "Common", CardFolderPath);
//        CreateWeaponCard("Dagger Slash", "Deals minor damage", "Common", CardFolderPath);
//        CreateWeaponCard("Arrow Shot", "Ranged attack, that has the chance of missing sometimes but critical damage if hit.", "Rare", CardFolderPath);
//        CreateWeaponCard("Spear Thrust", "Pierces enemy defences, dealing damage that ignores a portion of the armor.", "Epic", CardFolderPath);
//        CreateWeaponCard("Hammer Smash", "High-damage physical attack.", "Epic", CardFolderPath);
//        CreateWeaponCard("Sword Slash", "Deals moderate physical damage to a single enemy.", "Legendary", CardFolderPath);

//        // Generate Magic Cards
//        CreateMagicCard("Fireball", "Deals fire damage to a single enemy", "Common", CardFolderPath);
//        CreateMagicCard("Lightning Bolt", "Strikes an enemy with electricity", "Rare", CardFolderPath);
//        CreateMagicCard("Ice Spike", "Deals ice damage", "Rare", CardFolderPath);
//        CreateMagicCard("Earthquake", "Damages all enemies", "Epic", CardFolderPath);
//        CreateMagicCard("Chain Lightning", "Electric attack that jumps to multiple enemies.", "Legendary", CardFolderPath);
//        CreateMagicCard("Meteor Shower", "Heavy fire damage to all enemies Critical damage once charged up (like a set of turns).", "Legendary", CardFolderPath);

//        // Generate Defence Cards
//        CreateDefenceCard("Dodge", "Increases chance to evade the next attack.", "Common", CardFolderPath);
//        CreateDefenceCard("Shield Block", "Reduces incoming physical damage for one turn.", "Rare", CardFolderPath);
//        CreateDefenceCard("Magic Barrier", "Lowers damage from magical attacks for one turn.", "Epic", CardFolderPath);
//        CreateDefenceCard("Reflect", "Reflects a portion of magic damage at the attacker. (Only magic damage)", "Epic", CardFolderPath);
//        CreateDefenceCard("Total Defense", "Combines physical and magical defence for reduced damage from all sources.", "Legendary", CardFolderPath);

//        // Generate Healing Cards
//        CreateHealingCard("First Aid", "Heals a small amount of health to an ally only.", "Common", CardFolderPath);
//        CreateHealingCard("Healing Potion", "Restores a moderate amount of health to the user.", "Rare", CardFolderPath);
//        CreateHealingCard("Regen", "Heals a small amount of health for just you each turn for once turns.", "Epic", CardFolderPath);
//        CreateHealingCard("Group Heal", "Restores health to all allies only once.", "Legendary", CardFolderPath);

//        AssetDatabase.SaveAssets();
//        AssetDatabase.Refresh();
//        Debug.Log("Cards generated successfully!");
//    }

//    private static void CreateWeaponCard(string name, string description, string status, string folderPath)
//    {
//        WeaponCard card = ScriptableObject.CreateInstance<WeaponCard>();
//        card.name = name;
//        card.Initialize(name, description, status);
//        SaveCard(card, $"{folderPath}/{name}.asset");
//    }

//    private static void CreateMagicCard(string name, string description, string status, string folderPath)
//    {
//        MagicCard card = ScriptableObject.CreateInstance<MagicCard>();
//        card.name = name;
//        card.Initialize(name, description, status);
//        SaveCard(card, $"{folderPath}/{name}.asset");
//    }

//    private static void CreateDefenceCard(string name, string description, string status, string folderPath)
//    {
//        DefenceCard card = ScriptableObject.CreateInstance<DefenceCard>();
//        card.name = name;
//        card.Initialize(name, description, status);
//        SaveCard(card, $"{folderPath}/{name}.asset");
//    }

//    private static void CreateHealingCard(string name, string description, string status, string folderPath)
//    {
//        HealingCard card = ScriptableObject.CreateInstance<HealingCard>();
//        card.name = name;
//        card.Initialize(name, description, status);
//        SaveCard(card, $"{folderPath}/{name}.asset");
//    }

//    private static void SaveCard(ScriptableObject card, string path)
//    {
//        AssetDatabase.CreateAsset(card, path);
//        Debug.Log($"Created card: {path}");
//    }
//}
