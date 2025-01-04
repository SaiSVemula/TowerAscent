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
            UpdateObjectiveDisplay();
            Debug.Log($"Objective '{completedObjective}' marked as complete.");
        }
    }

    public bool IsObjectiveComplete(string objectiveName)
    {
        int index = objectives.IndexOf(objectiveName);
        return index >= 0 && index < currentObjectiveIndex;
    }
}
