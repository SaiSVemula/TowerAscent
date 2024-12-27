using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance of GameManager
    public static GameManager Instance;

    // Reference to the Player
    private Player playerInstance;

    // Game state variables
    private string SavedScene;
    private string NextScene; // New variable for the next scene
    private Vector3 PlayerCoord;
    private int CurrentHealth;
    private int CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName;

    private void Awake()
    {
        // Singleton pattern for GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
        }
    }

    // Initialize or retrieve the Player instance
    public Player GetPlayer()
    {
        if (playerInstance == null)
        {
            // Try to find an existing Player object in the scene
            playerInstance = FindObjectOfType<Player>();

            if (playerInstance == null)
            {
                // If no Player exists, create a new Player GameObject
                GameObject playerObject = new GameObject("Player");
                playerInstance = playerObject.AddComponent<Player>();

                // Set default Player values
                playerInstance.PlayerName = "Hero";
                playerInstance.Gold = 100;
                playerInstance.CurrentScene = SceneManager.GetActiveScene().name;
                playerInstance.CurrentHealth = 100; // Default health
            }

            DontDestroyOnLoad(playerInstance.gameObject); // Make the Player persistent
        }

        return playerInstance;
    }

    // Save the Player's state
    public void SavePlayerState()
    {
        if (playerInstance != null)
        {
            PlayerCoord = playerInstance.transform.position;
            CurrentHealth = playerInstance.CurrentHealth;
            CurrentCoins = playerInstance.Gold;
            CardsInInventory = playerInstance.Inventory.ToArray();
            PlayerName = playerInstance.PlayerName;

            Debug.Log("Player state saved.");
        }
    }

    // Restore the Player's state
    public void LoadPlayerState()
    {
        if (playerInstance != null)
        {
            playerInstance.transform.position = PlayerCoord;
            playerInstance.CurrentHealth = CurrentHealth;
            playerInstance.Gold = CurrentCoins;
            playerInstance.Inventory = new List<string>(CardsInInventory);
            playerInstance.PlayerName = PlayerName;

            Debug.Log("Player state loaded.");
        }
    }

    // Methods to manage the next scene
    public void SetNextScene(string sceneName)
    {
        NextScene = sceneName;
    }

    public string GetNextScene()
    {
        return NextScene;
    }

    // Methods to get single variables, for other scripts
    public string GetCurrentScene() { return SavedScene; }
    public Vector3 GetPlayerLocation() { return PlayerCoord; }
    public int GetPlayerHealth() { return CurrentHealth; }
    public int GetPlayerCoinCount() { return CurrentCoins; }
    public string[] GetPlayerCards() { return CardsInInventory; }
    public string GetPlayerName() { return PlayerName; }

    // Methods to set values on the GameManager
    public void UpdateCurrentScene() { SavedScene = SceneManager.GetActiveScene().name; }
    public void UpdatePlayerLocation(Vector3 location) { PlayerCoord = location; }
    public void UpdatePlayerHealth(int health) { CurrentHealth = health; }
    public void UpdatePlayerCoinCount(int coins) { CurrentCoins = coins; }
    public void UpdatePlayerCards(string[] cards) { CardsInInventory = cards; }
    public void UpdatePlayerName(string name) { PlayerName = name; }

    // Full get method, used when saving a game to perfs
    public (string, Vector3, int, int, string[], string) GetFullGameState()
    {
        return (SavedScene, PlayerCoord, CurrentHealth, CurrentCoins, CardsInInventory, PlayerName);
    }

    // Full update method, used when resuming a game and loading from perfs
    public void UpdateFullGameState(Transform playerTransform, int health, int coins, string[] cards, string name)
    {
        UpdatePlayerLocation(playerTransform.position);
        UpdatePlayerHealth(health);
        UpdatePlayerCoinCount(coins);
        UpdatePlayerCards(cards);
        UpdatePlayerName(name);
        UpdateCurrentScene();
        Debug.Log("Game state updated.");
    }

    // Used when clicking out of a menu mid gameplay
    public void LoadGameState()
    {
        SceneManager.LoadScene(SavedScene);
        if (playerInstance != null)
        {
            playerInstance.transform.position = PlayerCoord;
        }
    }

    // Loads a new scene and saves the current state
    public void LoadScene(string nextScene)
    {
        SavePlayerState();
        SceneManager.LoadScene(nextScene);
    }

    // Load the next scene set in the GameManager
    public void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(NextScene))
        {
            SavePlayerState();
            SceneManager.LoadScene(NextScene);
        }
        else
        {
            Debug.LogError("Next scene is not set!");
        }
    }

    // Reset Game State (resets all variables)
    public void Clear()
    {
        PlayerCoord = Vector3.zero;
        CurrentHealth = 100;
        CurrentCoins = 0;
        CardsInInventory = new string[0];
        SavedScene = "";
        NextScene = "";
        PlayerName = "";

        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
            playerInstance = null;
        }

        Debug.Log("Game state cleared.");
    }
}
