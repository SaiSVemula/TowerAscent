using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance for easy access across other scripts
    public static GameManager Instance;

    // Variables to store game data
    public string currentScene;
    public Vector3 playerLocation;
    public int playerHealth;
    public int playerCoinCount;
    public string[] playerCards;  // Example: storing player's cards as strings (you can modify this to your game logic)

    // Initialize Singleton and persist this object across scenes
    private void Awake()
    {
        // If an instance already exists, destroy this object
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Make this object persist across scene loads
        }
    }

    // Call this to save the game state
    public void SaveGameState(Transform playerTransform, int health, int coins, string[] cards)
    {
        // Save player data
        playerLocation = playerTransform.position;
        playerHealth = health;
        playerCoinCount = coins;
        playerCards = cards;

        // Save current scene
        currentScene = SceneManager.GetActiveScene().name;

        Debug.Log("Game state saved.");
    }

    public void SaveScene()
    {

        // Save current scene
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Call this to load the game state
    public void LoadGameState()
    {
        // Restore player data
        if (currentScene != "")
        {
            SceneManager.LoadScene(currentScene);
        }

        // Assuming you have a player object that you want to move back to the saved position
        Transform playerTransform = GameObject.FindWithTag("Player").transform;  // Replace "Player" with your player tag
        playerTransform.position = playerLocation;

        // Restore player health, coins, and cards (you can use these variables in your game logic)
        // Example:
        // playerHealth = loadedHealth;
        // playerCoinCount = loadedCoins;
        // playerCards = loadedCards;

        Debug.Log("Game state loaded.");
    }


    public void LoadScene()
    {
        SceneManager.LoadScene(currentScene);
    }

    // Call this when you want to reset game state (like on a new game)
    public void ResetGameState()
    {
        playerLocation = Vector3.zero;
        playerHealth = 100;  // Default health value
        playerCoinCount = 0;
        playerCards = new string[0];  // Empty array for cards
        currentScene = "";

        Debug.Log("Game state reset.");
    }
}
