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

    // Scene management
    private string SavedScene;
    private string previousScene;
    private string nextScene;
    public string PreviousScene { get => previousScene; set => previousScene = value; }
    public string NextScene { get => nextScene; set => nextScene = value; }



    private Vector3 PlayerCoord;
    private int CurrentHealth;
    private int CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName;

    private int minibattleWins;
    private int minibattleLosses;
    private int bigbattleWins;
    private int bigbattleLosses;

    private bool thirdperson;
    private float camsensitivity;

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

    // Methods to get single variables, for other scripts
    public string GetCurrentScene() { return SavedScene; }
    public Vector3 GetPlayerLocation() { return PlayerCoord; }
    public int GetPlayerHealth() { return CurrentHealth; }
    public int GetPlayerCoinCount() { return CurrentCoins; }
    public string[] GetPlayerCards() { return CardsInInventory; }
    public string GetPlayerName() { return PlayerName; }
    public int GetMinibattleWins() { return minibattleWins; }
    public int GetMinibattleLosses() { return minibattleLosses; }
    public int GetBigbattleWins() { return bigbattleWins; }
    public int GetBigbattleLosses() { return bigbattleLosses; }
    public bool GetPOV() { return thirdperson; } // Getter for PlayerName
    public float GetCamSensitivity() {return camsensitivity;}

    // Methods to set values on the GameManager
    public void UpdateCurrentScene() { SavedScene = SceneManager.GetActiveScene().name; }
    public void UpdatePlayerLocation(Vector3 location) { PlayerCoord = location; }
    public void UpdatePlayerHealth(int health) { CurrentHealth = health; }
    public void UpdatePlayerCoinCount(int coins) { CurrentCoins = coins; }
    public void UpdatePlayerCards(string[] cards) { CardsInInventory = cards; }
    public void UpdatePlayerName(string name) { PlayerName = name; }
    public void UpdateMinibattleWins(int wins) { minibattleWins = wins; }
    public void UpdateMinibattleLosses(int losses) { minibattleLosses = losses; }
    public void UpdateBigbattleWins(int wins) { bigbattleWins = wins; }
    public void UpdateBigbattleLosses(int losses) { bigbattleLosses = losses; }
    public void UpdatePOV(bool thirdprs) { thirdperson = thirdprs; }
    public void UpdateCamSensitivity(float sensitivity) {camsensitivity = sensitivity;}

    // Full get method, used when saving a game to perfs
    public (string, Vector3, int, int, string[], string, int, int, int, int) GetFullGameState()
    {
        return (SavedScene, PlayerCoord, CurrentHealth, CurrentCoins, CardsInInventory, PlayerName, minibattleWins, minibattleLosses, bigbattleWins, bigbattleLosses);
    }

    // Full update method, used when resuming a game and loading from perfs
    public void UpdateFullGameState(Transform playerTransform, int health, int coins, string[] cards, string name, int miniWins, int miniLosses, int bigWins, int bigLosses)
    {
        UpdatePlayerLocation(playerTransform.position);
        UpdatePlayerHealth(health);
        UpdatePlayerCoinCount(coins);
        UpdatePlayerCards(cards);
        UpdatePlayerName(name);
        UpdateMinibattleWins(miniWins);
        UpdateMinibattleLosses(miniLosses);
        UpdateBigbattleWins(bigWins);
        UpdateBigbattleLosses(bigLosses);
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
