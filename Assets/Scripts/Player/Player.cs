using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton instance of Player
    public static Player Instance;

    // Player Information
    public string PlayerName;
    public string CurrentScene;
    public string CurrentCheckpoint;
    public int Gold;
    public int MiniBattleWins;
    public int MiniBattleLosses;
    public int BigBattleWins;
    public int BigBattleLosses;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;


    // Inventory (placeholder for future implementation)
    public List<string> Inventory = new List<string>();

    // Objectives: Key-value pairs for objective names and their completion status
    public Dictionary<string, bool> Objectives = new Dictionary<string, bool>();

    // Calculate Total Battles
    public int TotalMiniBattles => MiniBattleWins + MiniBattleLosses;
    public int TotalBigBattles => BigBattleWins + BigBattleLosses;

    // Getters and setters for health
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    private void Awake()
    {
        maxHealth = 100;//deafult
        GameManager.Instance.UpdatePlayerHealth(maxHealth);
        currentHealth = maxHealth;
    }

    // Calculate Mini Battle Win/Loss Ratio
    public float GetMiniBattleWinLossRatio()
    {
        if (TotalMiniBattles > 0)
        {
            float ratio = (float)MiniBattleWins / TotalMiniBattles;
            Debug.Log($"Mini Battle Win/Loss Ratio: {ratio:F2}");
            return ratio;
        }

        Debug.Log("No mini battles fought. Win/Loss Ratio is 0.");
        return 0;
    }

    // Calculate Big Battle Win/Loss Ratio
    public float GetBigBattleWinLossRatio()
    {
        if (TotalBigBattles > 0)
        {
            float ratio = (float)BigBattleWins / TotalBigBattles;
            Debug.Log($"Big Battle Win/Loss Ratio: {ratio:F2}");
            return ratio;
        }

        Debug.Log("No big battles fought. Win/Loss Ratio is 0.");
        return 0;
    }

    // Add win/loss methods
    public void AddMiniBattleWin()
    {
        MiniBattleWins++;
        Debug.Log($"Mini battle won! Total wins: {MiniBattleWins}");
    }

    public void AddMiniBattleLoss()
    {
        MiniBattleLosses++;
        Debug.Log($"Mini battle lost! Total losses: {MiniBattleLosses}");
    }

    public void AddBigBattleWin()
    {
        BigBattleWins++;
        Debug.Log($"Big battle won! Total wins: {BigBattleWins}");
    }

    public void AddBigBattleLoss()
    {
        BigBattleLosses++;
        Debug.Log($"Big battle lost! Total losses: {BigBattleLosses}");
    }

    // Update gold
    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"Added {amount} gold. Total gold: {Gold}");
    }

    public void DeductGold(int amount)
    {
        Gold = Mathf.Max(0, Gold - amount);
        Debug.Log($"Deducted {amount} gold. Total gold: {Gold}");
    }

    // Objective management
    public void CompleteObjective(string objectiveName)
    {
        if (Objectives.ContainsKey(objectiveName))
        {
            Objectives[objectiveName] = true;
            Debug.Log($"Objective '{objectiveName}' marked as completed.");
        }
        else
        {
            Objectives.Add(objectiveName, true);
            Debug.Log($"New objective '{objectiveName}' added and marked as completed.");
        }
    }

    public bool IsObjectiveCompleted(string objectiveName)
    {
        return Objectives.ContainsKey(objectiveName) && Objectives[objectiveName];
    }

    // Debugging
    public void DebugStats()
    {
        Debug.Log($"Player: {PlayerName}, Gold: {Gold}");
        Debug.Log($"Mini Battles: Wins={MiniBattleWins}, Losses={MiniBattleLosses}, Total={TotalMiniBattles}, Win/Loss Ratio={GetMiniBattleWinLossRatio():F2}");
        Debug.Log($"Big Battles: Wins={BigBattleWins}, Losses={BigBattleLosses}, Total={TotalBigBattles}, Win/Loss Ratio={GetBigBattleWinLossRatio():F2}");
        Debug.Log($"Objectives: {string.Join(", ", Objectives.Keys)}");
    }
}
