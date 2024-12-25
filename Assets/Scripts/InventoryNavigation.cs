using UnityEngine;

public class InventoryNavigation : MonoBehaviour
{
    public RectTransform cardGrid; // Reference to the CardGrid
    public int visibleColumns = 4; // Number of visible columns at a time
    public float cardWidth = 170f; // Width of each card + spacing
    private int currentColumnIndex = 0; // Track current column

    public void ShowNextColumn()
    {
        // Calculate max scrollable columns
        int maxColumns = Mathf.CeilToInt((float)cardGrid.childCount / 2) - visibleColumns;
        if (currentColumnIndex < maxColumns)
        {
            currentColumnIndex++;
            UpdateGridPosition();
        }
    }

    public void ShowPreviousColumn()
    {
        if (currentColumnIndex > 0)
        {
            currentColumnIndex--;
            UpdateGridPosition();
        }
    }

    private void UpdateGridPosition()
    {
        float targetX = currentColumnIndex * cardWidth;
        cardGrid.anchoredPosition = new Vector2(-targetX, cardGrid.anchoredPosition.y);
    }
}
