using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private List<string> objectives = new List<string>();
    private int currentObjectiveIndex = 0;

    private void Start()
    {
        UpdateObjectiveDisplay();
        // Subscribe to the SpiderDefeated event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpiderDefeated += OnSpiderDefeated;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpiderDefeated -= OnSpiderDefeated;
        }
    }

    private void UpdateObjectiveDisplay()
    {
        objectivesText.text = currentObjectiveIndex < objectives.Count
            ? objectives[currentObjectiveIndex]
            : "Complete!";
    }

    public void CompleteCurrentObjective()
    {
        if (currentObjectiveIndex < objectives.Count)
        {
            string completedObjective = objectives[currentObjectiveIndex];
            currentObjectiveIndex++;

            // Check if the current objective is "Talk to NPC3"
            if (completedObjective == "Talk to NPC3")
            {
                Debug.Log("Objective 'Talk to NPC3' completed. Updating to 'Defeat the final boss!'");
                if (currentObjectiveIndex < objectives.Count)
                {
                    objectives[currentObjectiveIndex] = "Defeat the final boss!";
                    UpdateObjectiveDisplay();
                }
            }
            else
            {
                Debug.Log($"Objective '{completedObjective}' marked as complete.");
            }
        }
        UpdateObjectiveDisplay();
    }



    public bool IsObjectiveComplete(string objectiveName)
    {
        int index = objectives.IndexOf(objectiveName);
        return index >= 0 && index < currentObjectiveIndex;
    }

    public void SetObjectives(List<string> newObjectives)
    {
        objectives = newObjectives;
        currentObjectiveIndex = 0;
        UpdateObjectiveDisplay();
    }

    public void SetCurrentObjectiveIndex(int index)
    {
        currentObjectiveIndex = index;
        UpdateObjectiveDisplay();
    }


    public string GetCurrentObjective()
    {
        return currentObjectiveIndex < objectives.Count ? objectives[currentObjectiveIndex] : null;
    }


    // Triggered when the spider is defeated
    private void OnSpiderDefeated(string spiderID)
    {
        Debug.Log($"Spider with ID {spiderID} defeated. Checking objectives.");
        if (currentObjectiveIndex < objectives.Count && objectives[currentObjectiveIndex] == "Find and Defeat the Spider!")
        {
            CompleteCurrentObjective(); // Automatically complete the spider objective
            Debug.Log($"Objective 'Find and Defeat the Spider!' completed.");
        }
    }
}