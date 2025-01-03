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
            currentObjectiveIndex++;
            UpdateObjectiveDisplay();
        }
    }

    public void ClearAndSetNextObjective(string nextObjective)
    {
        objectives.Clear();
        objectives.Add(nextObjective);
        currentObjectiveIndex = 0;
        UpdateObjectiveDisplay();
    }

    public bool IsObjectiveComplete(string objectiveName)
    {
        return objectives.IndexOf(objectiveName) < currentObjectiveIndex;
    }
}
