using UnityEngine;

public class Shopkeeper : GeneralNPC
{
    // Uncomment this when you implement the shop menu
    // public GameObject shopMenu; // Reference to the shop menu UI

    protected override void Start()
    {
        base.Start(); // Call the base class's Start method to maintain its behavior
        // Uncomment this when shop menu is added
        /*
        if (shopMenu != null)
        {
            shopMenu.SetActive(false); // Ensure the shop menu is initially hidden
            Debug.Log("Shopkeeper-specific setup initialized, shop menu hidden.");
        }
        else
        {
            Debug.LogWarning("Shop menu is not assigned in the Inspector.");
        }
        */
        Debug.Log("Shopkeeper-specific setup initialized.");
    }

    public override void Interact()
    {
        base.Interact(); // Optionally call the base interaction logic
        Debug.Log("Welcome to the shop! (Shop menu would open here.)");
        // Uncomment this when shop menu is added
        /*
        if (shopMenu != null)
        {
            shopMenu.SetActive(true); // Show the shop menu when interacting
            Time.timeScale = 0f; // Pause the game while in the shop
            Debug.Log("Shop menu opened.");
        }
        else
        {
            Debug.LogError("Shop menu is missing. Please assign it in the Inspector.");
        }
        */
    }

    public void CloseShop()
    {
        // Uncomment this when shop menu is added
        /*
        if (shopMenu != null)
        {
            shopMenu.SetActive(false); // Hide the shop menu
            Time.timeScale = 1f; // Resume the game
            Debug.Log("Shop menu closed.");
        }
        else
        {
            Debug.LogWarning("Shop menu is not assigned in the Inspector.");
        }
        */
        Debug.Log("Shop closed function called, but no shop menu implemented yet.");
    }
}
