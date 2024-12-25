using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // makes a single instance of the gamemanager
    public static GameManager Instance;
    // variables used to hold crucial information
    
    private string SavedScene;
    private Vector3 PlayerCoord;
    private int CurrentHealth;
    private int CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName; // New variable to store the player's name

    private void Awake() // singleton
    {
        if (Instance != null && Instance != this){Destroy(gameObject);}
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Methods to get single variables, for other scripts.
    public string GetCurrentScene(){return SavedScene;}
    public Vector3 GetPlayerLocation(){return PlayerCoord;}
    public int GetPlayerHealth(){return CurrentHealth;}
    public int GetPlayerCoinCount(){return CurrentCoins;}
    public string[] GetPlayerCards(){return CardsInInventory;}
    public string GetPlayerName(){return PlayerName;} // Getter for PlayerName

    // Methods To set values on the gamemanager
    public void UpdateCurrentScene(){SavedScene = SceneManager.GetActiveScene().name;}
    public void UpdatePlayerLocation(Vector3 location){PlayerCoord = location;}
    public void UpdatePlayerHealth(int health){CurrentHealth = health;}
    public void UpdatePlayerCoinCount(int coins){CurrentCoins = coins;}
    public void UpdatePlayerCards(string[] cards){CardsInInventory = cards;}
    public void UpdatePlayerName(string name){PlayerName = name;} // Setter for PlayerName

    // full get method, used when saving a game to perfs
    public (string, Vector3, int, int, string[], string) GetFullGameState()
    {
        return (SavedScene, PlayerCoord, CurrentHealth, CurrentCoins, CardsInInventory, PlayerName);
    }

    // full update method, used when resuming a game and loading from perfs here
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

    // used when clicking out of a menu mid gameplay
    public void LoadGameState()
    {
        SceneManager.LoadScene(SavedScene);
        Transform playerTransform = GameObject.FindWithTag("Player")?.transform;
        playerTransform.position = PlayerCoord;
    }

    // loads only the scene (used for when you're on the main start screen and going back from settings page)
    public void LoadScene(){SceneManager.LoadScene(SavedScene);}

    // Reset Game State (resets all variables)
    public void Clear()
    {
        PlayerCoord = Vector3.zero;
        CurrentHealth = 100;
        CurrentCoins = 0;
        CardsInInventory = new string[0];
        SavedScene = "";
        PlayerName = ""; // Reset PlayerName
    }
}
