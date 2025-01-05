using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    public GameObject inventoryPanel;    // Reference to InventoryPanel
    public GameObject shopMenuPanel;    // Reference to ShopMenuPanel
    private string settingsSceneName = "SettingsPage"; // Replace with the exact name of your Settings Scene

    // Singleton instance for global access
    public static PanelManager Instance;

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

    public void OpenInventory()
    {
        CloseAllPanels(); // Close all panels before opening inventory
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
        }
    }

    public void OpenShopMenu()
    {
        CloseAllPanels(); // Close all panels before opening shop menu
        if (shopMenuPanel != null)
        {
            shopMenuPanel.SetActive(true);
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
    }

}