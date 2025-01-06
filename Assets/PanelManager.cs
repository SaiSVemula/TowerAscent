using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    public GameObject inventoryPanel;    // Reference to InventoryPanel
    public GameObject shopMenuPanel;    // Reference to ShopMenuPanel
    private string settingsSceneName = "SettingsPage"; // Replace with the exact name of your Settings Scene

    public static PanelManager Instance; // Singleton instance
    private bool isPlayerMovementPaused = false; // Track if player movement is paused

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsPlayerMovementPaused()
    {
        return isPlayerMovementPaused;
    }

    public void PausePlayerMovement()
    {
        isPlayerMovementPaused = true;
    }

    public void ResumePlayerMovement()
    {
        isPlayerMovementPaused = false;
    }

    public void OpenInventory()
    {
        CloseAllPanels(); // Close all panels before opening inventory
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            PausePlayerMovement();
        }
    }

    public void OpenShopMenu()
    {
        CloseAllPanels(); // Close all panels before opening shop menu
        if (shopMenuPanel != null)
        {
            shopMenuPanel.SetActive(true);
            PausePlayerMovement();
        }
    }

    public void OpenSettingsScene()
    {
        CloseAllPanels(); // Close all panels in the current scene
        SceneManager.LoadScene(settingsSceneName);
    }

    public void CloseAllPanels()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            Debug.Log("Inventory panel closed.");
        }
        if (shopMenuPanel != null)
        {
            shopMenuPanel.SetActive(false);
            Debug.Log("Shop menu panel closed.");
        }

        ResumePlayerMovement(); // Resume movement when all panels are closed
    }
}
