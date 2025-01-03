//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class LoadManager : MonoBehaviour
//{
//    public static void LoadGameState()
//    {
//        if (PlayerPrefs.HasKey("SavedScene"))
//        {
//            // Load the saved scene
//            string savedScene = PlayerPrefs.GetString("SavedScene");
//            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
//            levelLoader.LoadScene("StartPage", savedScene);
//            GameManager.Instance.LoadGameState();

//            // Start a coroutine to wait until the scene is fully loaded before adjusting the player's position
//            GameManager.Instance.StartCoroutine(WaitForSceneLoad());
//        }
//        else
//        {
//            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
//            levelLoader.LoadScene("StartPage", "StartPage");
//            Debug.LogWarning("No saved scene found in PlayerPrefs.");
//        }
//    }

//    private static IEnumerator WaitForSceneLoad()
//    {
//        // Wait until the scene is loaded and the player object is available
//        yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null);

//        // Find the player object by tag
//        Transform playerTransform = GameObject.FindWithTag("Player").transform;

//        // Retrieve the saved coordinates from PlayerPrefs
//        float playerCoordX = PlayerPrefs.GetFloat("PlayerCoordX", 0);
//        float playerCoordY = PlayerPrefs.GetFloat("PlayerCoordY", 0);
//        float playerCoordZ = PlayerPrefs.GetFloat("PlayerCoordZ", 0);

//        // Set the player's position to the saved coordinates
//        playerTransform.position = new Vector3(playerCoordX, playerCoordY, playerCoordZ);

//        // Log the loaded position
//        Debug.Log($"Player position loaded: {playerTransform.position}");
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    public static void LoadGameState()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            // Load the saved scene
            string savedScene = PlayerPrefs.GetString("SavedScene");
            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            levelLoader.LoadScene("StartPage", savedScene);

            // Restore game state
            GameManager.Instance.LoadGameState();

            // Start a coroutine to wait until the scene is fully loaded before adjusting the player's position
            GameManager.Instance.StartCoroutine(WaitForSceneLoad());
        }
        else
        {
            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            levelLoader.LoadScene("StartPage", "StartPage");
            Debug.LogWarning("No saved scene found in PlayerPrefs.");
        }
    }

    private static IEnumerator WaitForSceneLoad()
    {
        // Wait until the scene is loaded and the player object is available
        yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null);

        // Find the player object by tag
        Transform playerTransform = GameObject.FindWithTag("Player").transform;

        // Retrieve saved player state and update GameManager
        GameManager gameManager = GameManager.Instance;

        // Restore position
        float playerCoordX = PlayerPrefs.GetFloat("PlayerCoordX", 0);
        float playerCoordY = PlayerPrefs.GetFloat("PlayerCoordY", 0);
        float playerCoordZ = PlayerPrefs.GetFloat("PlayerCoordZ", 0);
        playerTransform.position = new Vector3(playerCoordX, playerCoordY, playerCoordZ);

        // Restore health
        int savedHealth = PlayerPrefs.GetInt("CurrentHealth", 100);
        gameManager.UpdatePlayerHealth(savedHealth);

        // Restore coins
        int savedCoins = PlayerPrefs.GetInt("CurrentCoins", 0);
        gameManager.UpdatePlayerCoinCount(savedCoins);

        // Restore name
        string savedName = PlayerPrefs.GetString("PlayerName", "Hero");
        gameManager.UpdatePlayerName(savedName);

        // Restore inventory (ensure this matches your inventory system)
        // Example: Deserialize saved inventory strings if needed
        string savedInventoryRaw = PlayerPrefs.GetString("PlayerInventory", "");
        string[] inventoryItems = string.IsNullOrEmpty(savedInventoryRaw) ? new string[0] : savedInventoryRaw.Split(',');
        gameManager.UpdatePlayerCards(inventoryItems);

        // Restore additional game state like wins/losses
        gameManager.UpdateMinibattleWins(PlayerPrefs.GetInt("MinibattleWins", 0));
        gameManager.UpdateMinibattleLosses(PlayerPrefs.GetInt("MinibattleLosses", 0));
        gameManager.UpdateBigbattleWins(PlayerPrefs.GetInt("BigbattleWins", 0));
        gameManager.UpdateBigbattleLosses(PlayerPrefs.GetInt("BigbattleLosses", 0));

        Debug.Log($"Game state restored: Health={savedHealth}, Coins={savedCoins}, Position={playerTransform.position}, Inventory={savedInventoryRaw}");
    }
}
