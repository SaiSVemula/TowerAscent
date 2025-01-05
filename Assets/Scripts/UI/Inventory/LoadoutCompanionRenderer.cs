using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadoutCompanionsRenderer : MonoBehaviour
{
    private bool isRendered = false; // Track if the companions are already rendered

    public void RenderCompanions(List<CompanionCard> ownedCompanions, Transform grid)
    {
        if (isRendered) return; // Skip rendering if already done

        foreach (var companion in ownedCompanions)
        {
            if (companion == null) continue;

            // Create a new companion GameObject
            GameObject newCompanion = new GameObject(companion.CompanionName, typeof(RectTransform), typeof(Image), typeof(CompanionCardDisplay));

            // Set the parent to the grid
            newCompanion.transform.SetParent(grid, false);

            // Configure RectTransform
            RectTransform rect = newCompanion.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 200); // Companion card size
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            // Add and configure Image component
            Image img = newCompanion.GetComponent<Image>();
            img.sprite = companion.CompanionSprite;
            img.color = Color.white;

            // Add DraggableItem component
            newCompanion.AddComponent<DraggableItem>();

            // Assign the CompanionCard data to the CompanionCardDisplay
            var companionDisplay = newCompanion.GetComponent<CompanionCardDisplay>();
            companionDisplay.Initialize(companion);
        }

        isRendered = true; // Mark as rendered
    }
}
