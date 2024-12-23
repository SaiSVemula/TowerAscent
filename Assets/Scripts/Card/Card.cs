using UnityEngine;

public abstract class Card : ScriptableObject
{
    [SerializeField] private string name; 
    [SerializeField] private string description;
    private bool isClicked; // Not serialized as used at runtime only

    public string Name => name;
    public string Description => description;

    // Called when the card is clicked or interacted with.
    private void OnMouseDown()
    {
        isClicked = true;
        Debug.Log($"Card {Name} was clicked.");
    }

    // Returns whether the card has been clicked and resets the state.
    public bool IsClicked()
    {
        if (isClicked)
        {
            isClicked = false; // Reset the state after detecting
            return true;
        }
        return false;
    }

    // Abstract method to define the card's effect.
    public abstract void Use(Player player, Enemy enemy);
}
