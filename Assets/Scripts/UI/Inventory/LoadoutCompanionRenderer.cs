using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadoutCompanionsRenderer : MonoBehaviour
{
    public void RenderCompanions(List<CompanionCard> ownedCompanions, Transform grid)
    {
        foreach (var companion in ownedCompanions)
        {
            if (companion == null) continue;

            // Create a new companion GameObject
            GameObject newCompanion = new GameObject(companion.CompanionName, typeof(RectTransform), typeof(Image));

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

            // Add a child for the companion's text
            GameObject textObj = new GameObject("CompanionText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(newCompanion.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(150, 30); // Text size
            textRect.anchoredPosition = new Vector2(0, -85); // Position below the companion card

            // Configure the Text component
            Text text = textObj.GetComponent<Text>();
            text.text = companion.CompanionName;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
        }
    }
}
