using System.Collections.Generic;
using System.Linq;
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

    // Difficulty
    public Difficulty GameDifficulty { get; set; }
    public int CurrentCoins1 { get => CurrentCoins; set => CurrentCoins = value; }
    public List<Card> CurrentCardLoadout { get => cardLoadout; set => cardLoadout = value; }

    private List<GameObject> partyCompanions = new List<GameObject>(); // Store companions in the party
    public List<GameObject> PartyCompanions => partyCompanions;

    private List<CompanionCard> ownedCompanions = new List<CompanionCard>();

    public List<CompanionCard> GetOwnedCompanions()
    {
        return new List<CompanionCard>(ownedCompanions); // Return a copy to prevent direct modification
    }

    public void AddCompanion(CompanionCard companion)
    {
        if (companion != null)
        {
            ownedCompanions.Add(companion);
            Debug.Log($"Companion added to inventory: {companion.name}");
        }
        else
        {
            Debug.LogWarning("Tried to add a null companion to inventory.");
        }
    }

    // Mini-battle card pool
    private List<Card> miniBattleCardPool = new List<Card>();

    // Track defeated spiders
    private HashSet<string> defeatedSpiders = new HashSet<string>();
    public HashSet<string> DefeatedSpiders => defeatedSpiders;

    private bool hasGameStateLoaded = false;
    public bool HasGameStateLoaded => hasGameStateLoaded;

    private Vector3 PlayerCoord;
    private int CurrentHealth;
    private int CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName;
    private List<Card> cardLoadout;

    private int minibattleWins;
    private int minibattleLosses;
    private int bigbattleWins;
    private int bigbattleLosses;

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
            LoadSpiderStates();
        }
        SceneManager.sceneLoaded += SceneLoadup;
    }


    // Add a companion to the party
    public void AddCompanionToParty(GameObject companion)
    {
        if (!partyCompanions.Contains(companion))
        {
            partyCompanions.Add(companion);
            DontDestroyOnLoad(companion); // Persist companion across scenes
            Debug.Log($"Companion {companion.name} added to the party.");
        }
    }

    // Spawn all companions in the current level
    public void SpawnCompanionsInCurrentLevel()
    {
        foreach (var companion in partyCompanions)
        {
            if (companion != null)
            {
                // Position companions near the player
                var player = FindObjectOfType<Player>();
                if (player != null)
                {
                    companion.transform.position = player.transform.position + new Vector3(Random.Range(1, 3), 0, Random.Range(1, 3));
                }
                companion.SetActive(true); // Ensure companion is active
            }
        }
    }

    public void SetCompanionType(CompanionType companionType)
    {
        PlayerPrefs.SetInt("PlayerCompanionType", (int)companionType);
        PlayerPrefs.Save();
        Debug.Log($"Companion type set to: {companionType}");
    }

    public CompanionType GetCompanionType()
    {
        int companionTypeInt = PlayerPrefs.GetInt("PlayerCompanionType", 0); // Default to Companion1
        return (CompanionType)companionTypeInt;
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
            LoadMiniBattleCardPoolFromPrefs();

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

    public void MarkSpiderDefeated(string spiderID)
    {
        if (!defeatedSpiders.Contains(spiderID))
        {
            defeatedSpiders.Add(spiderID);
            Debug.Log($"Spider with ID {spiderID} marked as defeated.");
        }
    }

    public bool IsSpiderDefeated(string spiderID)
    {
        Debug.Log($"Checking if spider with ID {spiderID} is defeated: {defeatedSpiders.Contains(spiderID)}");
        return defeatedSpiders.Contains(spiderID);
    }


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
        // Perform game state loading logic here
        SceneManager.LoadScene(SavedScene);

        if (playerInstance != null)
        {
            playerInstance.transform.position = PlayerCoord;
            CurrentHealth = PlayerPrefs.GetInt("CurrentHealth", 100);
            playerInstance.Gold = CurrentCoins;
            playerInstance.Inventory = new List<string>(CardsInInventory);
            playerInstance.PlayerName = PlayerName;

            Debug.Log("Game state loaded.");
        }

        // Mark the game state as loaded
        hasGameStateLoaded = true;
    }

    // Loads a new scene and saves the current state
    public void LoadScene(string nextScene)
    {
        SavePlayerState();
        SceneManager.LoadScene(nextScene);
    }

    // Clears the game state
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

        defeatedSpiders.Clear(); // Reset spider states
        PlayerPrefs.DeleteKey("DefeatedSpiders"); // Clear saved spider states
        Debug.Log("Game state cleared.");
    }


    // Post Mini Battle logic.
    public void RewardMiniBattleGold(int rewardAmount)
    {
        UpdatePlayerCoinCount(CurrentCoins1 + rewardAmount);
        Debug.Log($"Player rewarded with {rewardAmount} gold for mini-battle victory. Total coins: {CurrentCoins1}");
    }

    public void AddMiniBattleWin(string spiderID, int rewardAmount)
    {
        minibattleWins++;
        RewardMiniBattleGold(rewardAmount);
        MarkSpiderDefeated(spiderID); // Ensure spider is marked as defeated
        Debug.Log($"Mini-battle win recorded. Spider defeated: {spiderID}. Total wins: {minibattleWins}");
    }

    public void AddMiniBattleLoss()
    {
        minibattleLosses++;
        Debug.Log($"Mini-battle loss recorded. Total losses: {minibattleLosses}");
    }

    // Save spider state to PlayerPrefs
    public void SaveSpiderStates()
    {
        string savedSpiders = string.Join(",", defeatedSpiders);
        PlayerPrefs.SetString("DefeatedSpiders", savedSpiders);
        PlayerPrefs.Save();
        Debug.Log("Spider defeat states saved.");
    }

    // Load spider state from PlayerPrefs
    public void LoadSpiderStates()
    {
        string savedSpiders = PlayerPrefs.GetString("DefeatedSpiders", "");
        if (!string.IsNullOrEmpty(savedSpiders))
        {
            string[] spiderIDs = savedSpiders.Split(',');
            defeatedSpiders = new HashSet<string>(spiderIDs);
        }
        Debug.Log("Spider defeat states loaded.");
    }

    // Mini-battle card pool management

    public List<Card> GetMiniBattleCardPool()
    {
        if (miniBattleCardPool == null)
        {
            miniBattleCardPool = new List<Card>();
            Debug.LogWarning("MiniBattleCardPool was null. Initialized with an empty list.");
        }
        return miniBattleCardPool;
    }

    public void UpdateMiniBattleCardPool(List<Card> newPool)
    {
        miniBattleCardPool = newPool;
        SaveMiniBattleCardPoolToPrefs();
        Debug.Log("Mini-battle card pool updated and saved.");
    }

    public void AddDefaultCardIfPoolEmpty()
    {
        if (miniBattleCardPool == null || miniBattleCardPool.Count == 0)
        {
            Debug.LogWarning("MiniBattleCardPool is empty. Adding default card.");
            var defaultCard = Resources.Load<Card>("Cards/Weapon Cards/Axe Chop");
            if (defaultCard != null)
            {
                miniBattleCardPool.Add(defaultCard);
                Debug.Log("Default card (Axe Chop) added to MiniBattleCardPool.");
            }
            else
            {
                Debug.LogError("Default weapon card (Axe Chop) not found in Resources.");
            }
        }
    }

    public void SaveMiniBattleCardPoolToPrefs()
    {
        List<string> cardNames = miniBattleCardPool.Select(card => card.Name).ToList();
        string serializedPool = string.Join(",", cardNames);
        PlayerPrefs.SetString("MiniBattleCardPool", serializedPool);
        PlayerPrefs.Save();
        Debug.Log("Mini-battle card pool saved to PlayerPrefs.");
    }

    public void LoadMiniBattleCardPoolFromPrefs()
    {
        string serializedPool = PlayerPrefs.GetString("MiniBattleCardPool", "");
        if (!string.IsNullOrEmpty(serializedPool))
        {
            string[] cardNames = serializedPool.Split(',');
            miniBattleCardPool = cardNames
                .Select(cardName => Resources.Load<Card>($"Cards/{cardName}"))
                .Where(card => card != null)
                .ToList();
            Debug.Log("Mini-battle card pool loaded from PlayerPrefs.");
        }
        else
        {
            Debug.LogWarning("No saved mini-battle card pool found in PlayerPrefs.");
        }
    }

    public void CompleteObjective(string objectiveName)
    {
        // Implement objective completion logic here
    }



    private void SceneLoadup(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        // Check and update particle and post-processing states
        UpdateParticlesState();
        UpdatePostProcessingState();
    }

    private void UpdateParticlesState()
    {
        // Check the PlayerPref key "AreParticlesOn"
        bool areParticlesOn = PlayerPrefs.GetInt("AreParticlesOn", 1) == 1; // Default to true (1) if key does not exist

        // Find all objects tagged as "Particle"
        GameObject[] particleObjects = GameObject.FindGameObjectsWithTag("particle");

        // Enable or disable them based on the PlayerPref value
        foreach (GameObject particleObject in particleObjects)
        {
            particleObject.SetActive(areParticlesOn);
        }

        Debug.Log($"Particles are now {(areParticlesOn ? "enabled" : "disabled")}.");
    }

    private void UpdatePostProcessingState()
    {
        // Check the PlayerPref key "AreEffectsOn"
        bool areEffectsOn = PlayerPrefs.GetInt("AreEffectsOn", 1) == 1; // Default to true (1) if key does not exist

        // Find all objects with the tag "effects"
        GameObject[] effectsObjects = GameObject.FindGameObjectsWithTag("effects");
        if (effectsObjects.Length > 0)
        {
            foreach (GameObject effectsObject in effectsObjects)
            {
                effectsObject.SetActive(areEffectsOn);
            }

            Debug.Log($"Effects are now {(areEffectsOn ? "enabled" : "disabled")}.");
        }
        else
        {
            Debug.LogWarning("No objects with the tag 'effects' found in the scene.");
        }
    }
}
