using UnityEngine;
using UnityEditor;

public class CardGenerator : MonoBehaviour
{
    [MenuItem("Tools/Generate All Cards")]
    public static void GenerateAllCards()
    {
        // Define the folder to save the cards
        string folderPath = "Assets/Cards";

        // Ensure the folder exists
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Cards");
        }

        // Create Weapon Cards
        CreateWeaponCard("Sword Slash", "Deals moderate physical damage to a single enemy.", 15, folderPath);
        CreateWeaponCard("Axe Chop", "Deals moderate physical damage to a single enemy.", 20, folderPath);
        CreateWeaponCard("Arrow Shot", "Ranged attack, that has a chance of missing but deals critical damage if it hits.", 25, folderPath);
        CreateWeaponCard("Spear Thrust", "Pierces enemy defenses, dealing damage that ignores a portion of armor.", 30, folderPath);
        CreateWeaponCard("Hammer Smash", "High-damage physical attack.", 35, folderPath);
        CreateWeaponCard("Whirlwind", "Deals physical damage to all enemies.", 20, folderPath);
        CreateWeaponCard("Dagger", "Deals minor damage.", 10, folderPath);

        // Create Magic Cards
        CreateMagicCard("Fireball", "Deals fire damage to a single enemy.", 25, "Burn", folderPath);
        CreateMagicCard("Lightning Bolt", "Strikes an enemy with electricity.", 30, "Stun", folderPath);
        CreateMagicCard("Ice Spike", "Deals ice damage to a single enemy.", 20, "Freeze", folderPath);
        CreateMagicCard("Earthquake", "Damages all enemies.", 40, "", folderPath);
        CreateMagicCard("Chain Lightning", "Electric attack that jumps to multiple enemies.", 30, "", folderPath);
        CreateMagicCard("Meteor Shower", "Heavy fire damage to all enemies. Critical damage once charged up (like a set of turns).", 50, "", folderPath);
        CreateMagicCard("Sleep Spell", "Puts an enemy to sleep for one turn.", 0, "Sleep", folderPath);
        CreateMagicCard("Curse", "Reduces an enemy's attack and defense for several turns.", 0, "Weaken", folderPath);

        // Create Defense Cards
        CreateDefenseCard("Shield Block", "Reduces incoming physical damage for one turn.", 15, folderPath);
        CreateDefenseCard("Magic Barrier", "Lowers damage from magical attacks for one turn.", 20, folderPath);
        CreateDefenseCard("Dodge", "Increases chance to evade the next attack.", 0, folderPath);
        CreateDefenseCard("Reflect", "Reflects a portion of magic damage at the attacker.", 10, folderPath);
        CreateDefenseCard("Total Defense", "Combines physical and magical defense for reduced damage from all sources.", 25, folderPath);
        CreateDefenseCard("Stealth", "Avoids all attacks for one turn but cannot perform actions.", 0, folderPath);

        // Create Healing Cards
        CreateHealingCard("Healing Potion", "Restores a moderate amount of health to the user.", 25, false, folderPath);
        CreateHealingCard("First Aid", "Heals a small amount of health to an ally.", 15, false, folderPath);
        CreateHealingCard("Regeneration", "Heals a small amount of health for just you each turn for three turns.", 10, false, folderPath);
        CreateHealingCard("Group Regeneration", "Restores health to all allies for three turns.", 20, false, folderPath);
        CreateHealingCard("Group Heal", "Restores health to all allies only once.", 30, false, folderPath);
        CreateHealingCard("Revive", "Brings a fallen ally back to life with minimal health.", 0, true, folderPath);

        Debug.Log("All cards have been generated!");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateWeaponCard(string name, string description, int damage, string folderPath)
    {
        WeaponCard card = ScriptableObject.CreateInstance<WeaponCard>();
        var serializedObject = new SerializedObject(card);
        serializedObject.FindProperty("name").stringValue = name;
        serializedObject.FindProperty("description").stringValue = description;
        serializedObject.FindProperty("damage").intValue = damage;
        serializedObject.ApplyModifiedProperties();

        SaveCard(card, $"{folderPath}/{name}.asset");
    }

    private static void CreateMagicCard(string name, string description, int magicDamage, string effect, string folderPath)
    {
        MagicCard card = ScriptableObject.CreateInstance<MagicCard>();
        var serializedObject = new SerializedObject(card);
        serializedObject.FindProperty("name").stringValue = name;
        serializedObject.FindProperty("description").stringValue = description;
        serializedObject.FindProperty("magicDamage").intValue = magicDamage;
        serializedObject.FindProperty("effect").stringValue = effect;
        serializedObject.ApplyModifiedProperties();

        SaveCard(card, $"{folderPath}/{name}.asset");
    }

    private static void CreateDefenseCard(string name, string description, int defenseValue, string folderPath)
    {
        DefenseCard card = ScriptableObject.CreateInstance<DefenseCard>();
        var serializedObject = new SerializedObject(card);
        serializedObject.FindProperty("name").stringValue = name;
        serializedObject.FindProperty("description").stringValue = description;
        serializedObject.FindProperty("defenseValue").intValue = defenseValue;
        serializedObject.ApplyModifiedProperties();

        SaveCard(card, $"{folderPath}/{name}.asset");
    }

    private static void CreateHealingCard(string name, string description, int healingAmount, bool canRevive, string folderPath)
    {
        HealingCard card = ScriptableObject.CreateInstance<HealingCard>();
        var serializedObject = new SerializedObject(card);
        serializedObject.FindProperty("name").stringValue = name;
        serializedObject.FindProperty("description").stringValue = description;
        serializedObject.FindProperty("healingAmount").intValue = healingAmount;
        serializedObject.FindProperty("canRevive").boolValue = canRevive;
        serializedObject.ApplyModifiedProperties();

        SaveCard(card, $"{folderPath}/{name}.asset");
    }

    private static void SaveCard(Card card, string path)
    {
        AssetDatabase.CreateAsset(card, path);
        Debug.Log($"Created card: {card.Name} at {path}");
    }
}
