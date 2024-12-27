using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Makes a single instance of the GameManager
    public static GameManager Instance;

    // Variables used to hold crucial information
    private string SavedScene;
    private Vector3 PlayerCoord;
    private int CurrentHealth;
    private int CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName; // Stores the player's name

    // New variables for tracking battles
    private int minibattleWins;
    private int minibattleLosses;
    private int bigbattleWins;
    private int bigbattleLosses;

    private void Awake() // Singleton pattern
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Methods to get single variables, for other scripts
    public string GetCurrentScene() { return SavedScene; }
    public Vector3 GetPlayerLocation() { return PlayerCoord; }
    public int GetPlayerHealth() { return CurrentHealth; }
    public int GetPlayerCoinCount() { return CurrentCoins; }
    public string[] GetPlayerCards() { return CardsInInventory; }
    public string GetPlayerName() { return PlayerName; } // Getter for PlayerName

    // Getters for battle statistics
    public int GetMinibattleWins() { return minibattleWins; }
    public int GetMinibattleLosses() { return minibattleLosses; }
    public int GetBigbattleWins() { return bigbattleWins; }
    public int GetBigbattleLosses() { return bigbattleLosses; }

    // Methods to set values on the GameManager
    public void UpdateCurrentScene() { SavedScene = SceneManager.GetActiveScene().name; }
    public void SetCurrentScene(string Scene) { SavedScene = Scene; }
    public void UpdatePlayerLocation(Vector3 location) { PlayerCoord = location; }
    public void UpdatePlayerHealth(int health) { CurrentHealth = health; }
    public void UpdatePlayerCoinCount(int coins) { CurrentCoins = coins; }
    public void UpdatePlayerCards(string[] cards) { CardsInInventory = cards; }
    public void UpdatePlayerName(string name) { PlayerName = name; } // Setter for PlayerName

    // Setters for battle statistics
    public void UpdateMinibattleWins(int wins) { minibattleWins = wins; }
    public void UpdateMinibattleLosses(int losses) { minibattleLosses = losses; }
    public void UpdateBigbattleWins(int wins) { bigbattleWins = wins; }
    public void UpdateBigbattleLosses(int losses) { bigbattleLosses = losses; }

    // Full get method, used when saving a game to prefs
    public (string, Vector3, int, int, string[], string, int, int, int, int) GetFullGameState()
    {
        return (SavedScene, PlayerCoord, CurrentHealth, CurrentCoins, CardsInInventory, PlayerName, minibattleWins, minibattleLosses, bigbattleWins, bigbattleLosses);
    }

    // Full update method, used when resuming a game and loading from prefs here
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
        // Load the saved scene
        SceneManager.LoadScene(SavedScene);

        // Find the player object by tag
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;

        // Check if the player transform was found and then set the position
        if (playerTransform != null)
        {
            playerTransform.position = PlayerCoord;
            Debug.Log($"Player position loaded: {playerTransform.position}");
        }
        else
        {
            Debug.LogWarning("Player object not found!");
        }
    }


    // Loads only the scene (used for when you're on the main start screen and going back from settings page)
    public void LoadScene() { SceneManager.LoadScene(SavedScene); }

    // Reset Game State (resets all variables)
    public void Clear()
    {
        PlayerCoord = Vector3.zero;
        CurrentHealth = 100;
        CurrentCoins = 0;
        CardsInInventory = new string[0];
        SavedScene = "";
        PlayerName = ""; // Reset PlayerName
        minibattleWins = 0;
        minibattleLosses = 0;
        bigbattleWins = 0;
        bigbattleLosses = 0;
    }
}
