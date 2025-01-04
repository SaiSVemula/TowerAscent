using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryCompanionsRenderer : MonoBehaviour
{
    [SerializeField] private Transform companionGrid; // Reference to the CompanionGrid
    [SerializeField] private Sprite defaultCompanionSprite; // Placeholder sprite for companions
    [SerializeField] private Text noCompanionsText; // Text to show when no companions are available

    private void OnEnable()
    {
        RefreshCompanionsUI();
    }

    public void RefreshCompanionsUI()
    {
        ClearCompanionsUI();

        List<CompanionCard> ownedCompanions = GameManager.Instance.GetOwnedCompanions();

        if (ownedCompanions == null || ownedCompanions.Count == 0)
        {
            noCompanionsText.gameObject.SetActive(true);
            return;
        }

        noCompanionsText.gameObject.SetActive(false);

        foreach (var companion in ownedCompanions)
        {
            if (companion == null) continue;

            // Create a new companion GameObject
            GameObject newCompanion = new GameObject(companion.Name, typeof(RectTransform), typeof(Image));

            // Set the parent to the CompanionGrid
            newCompanion.transform.SetParent(companionGrid, false);

            // Configure RectTransform
            RectTransform rect = newCompanion.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 200); // Companion size
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            // Add and configure Image component
            Image img = newCompanion.GetComponent<Image>();
            img.sprite = companion.Sprite ?? defaultCompanionSprite; // Use the companion's sprite or default
            img.color = Color.white;

            // Add a child for the companion's text
            GameObject textObj = new GameObject("CompanionText", typeof(RectTransform), typeof(Text));
            textObj.transform.SetParent(newCompanion.transform, false);
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(150, 30); // Text size
            textRect.anchoredPosition = new Vector2(0, -85); // Position below the companion

            // Configure the Text component
            Text text = textObj.GetComponent<Text>();
            text.text = companion.Name; // Display companion name
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
        }
    }

    private void ClearCompanionsUI()
    {
        foreach (Transform child in companionGrid)
        {
            Destroy(child.gameObject);
        }
    }
}
