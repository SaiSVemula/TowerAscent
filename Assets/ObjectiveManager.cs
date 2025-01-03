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
        if (currentObjectiveIndex < objectives.Count)
        {
            objectivesText.text = objectives[currentObjectiveIndex];
        }
        else
        {
            objectivesText.text = "Complete!";
        }
    }

    public void CompleteCurrentObjective()
    {
        if (currentObjectiveIndex < objectives.Count)
        {
            currentObjectiveIndex++;
            UpdateObjectiveDisplay();
        }
    }

    public void AddObjective(string objective)
    {
        objectives.Add(objective);
        if (objectives.Count == 1)
        {
            UpdateObjectiveDisplay();
        }
    }
}
