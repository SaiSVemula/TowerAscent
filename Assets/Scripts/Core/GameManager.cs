using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Holds one instance
    public static GameManager Instance;
    private Player playerInstance;
    public event Action<string> SpiderDefeated;

    // Scene flow
    private string SavedScene, previousScene, nextScene;
    public string PreviousScene { get => previousScene; set => previousScene = value; }
    public string NextScene { get => nextScene; set => nextScene = value; }

    // Core data
    public Difficulty GameDifficulty { get; set; }
    public int CurrentCoins1 { get => CurrentCoins; set => CurrentCoins = value; }
    public List<Card> CurrentCardLoadout { get => cardLoadout; set => cardLoadout = value; }
    private List<CompanionCard> ownedCompanions = new List<CompanionCard>();
    private List<Card> miniBattleCardPool = new List<Card>();
    private HashSet<string> defeatedSpiders = new HashSet<string>();
    public HashSet<string> DefeatedSpiders => defeatedSpiders;
    private bool hasGameStateLoaded = false;
    public bool HasGameStateLoaded => hasGameStateLoaded;

    // Player stats
    private Vector3 PlayerCoord;
    private int CurrentHealth, CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName;
    private List<Card> cardLoadout;

    // Battle counters
    private int minibattleWins, minibattleLosses, bigbattleWins, bigbattleLosses;

    // Sets up the manager
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSpiderStates();
        }
        SceneManager.sceneLoaded += SceneLoadup;
    }

    // Finds or creates the player
    public Player GetPlayer()
    {
        if (playerInstance == null)
        {
            playerInstance = FindObjectOfType<Player>();
            if (playerInstance == null)
            {
                GameObject playerObject = new GameObject("Player");
                playerInstance = playerObject.AddComponent<Player>();
                playerInstance.PlayerName = "Hero";
                playerInstance.Gold = 100;
                playerInstance.CurrentScene = SceneManager.GetActiveScene().name;
                playerInstance.CurrentHealth = 100;
            }
            DontDestroyOnLoad(playerInstance.gameObject);
        }
        return playerInstance;
    }

    // Saves player info
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

    // Loads player info
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

    // Getters for single variables
    public string GetCurrentScene() { return SavedScene; }
    public Vector3 GetPlayerLocation() { return PlayerCoord; }
    public int GetPlayerHealth() { return CurrentHealth; }
    public int GetPlayerCoinCount() { return CurrentCoins; }
    public string[] GetPlayerCards() { return CardsInInventory; }
    public List<CompanionCard> GetOwnedCompanions() { return new List<CompanionCard>(ownedCompanions); }
    public string GetPlayerName() { return PlayerName; }
    public int GetMinibattleWins() { return minibattleWins; }
    public int GetMinibattleLosses() { return minibattleLosses; }
    public int GetBigbattleWins() { return bigbattleWins; }
    public int GetBigbattleLosses() { return bigbattleLosses; }

    // Updaters for single variables
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

    // Marks spider as defeated
    public void MarkSpiderDefeated(string spiderID)
    {
        if (!defeatedSpiders.Contains(spiderID))
        {
            defeatedSpiders.Add(spiderID);
            Debug.Log($"Spider with ID {spiderID} marked as defeated.");
            SpiderDefeated?.Invoke(spiderID);
        }
    }

    // Checks if spider was defeated
    public bool IsSpiderDefeated(string spiderID)
    {
        Debug.Log($"Checking if spider with ID {spiderID} is defeated: {defeatedSpiders.Contains(spiderID)}");
        return defeatedSpiders.Contains(spiderID);
    }

    // Returns full snapshot
    public (string, Vector3, int, int, string[], string, int, int, int, int) GetFullGameState()
    {
        return (SavedScene, PlayerCoord, CurrentHealth, CurrentCoins, CardsInInventory,
                PlayerName, minibattleWins, minibattleLosses, bigbattleWins, bigbattleLosses);
    }

    // Updates multiple values at once
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

    // Add these new methods inside your GameManager class
    public void SaveOwnedCompanionsToPrefs()
    {
        List<string> companionNames = ownedCompanions.Select(c => c.CompanionName).ToList();
        string serializedCompanions = string.Join(",", companionNames);
        PlayerPrefs.SetString("OwnedCompanions", serializedCompanions);
        PlayerPrefs.Save();

        Debug.Log("Owned companions saved to PlayerPrefs.");
    }

    public void LoadOwnedCompanionsFromPrefs()
    {
        string serializedCompanions = PlayerPrefs.GetString("OwnedCompanions", "");

        if (string.IsNullOrEmpty(serializedCompanions))
        {
            Debug.Log("No owned companions found in PlayerPrefs.");
            return;
        }

        string[] companionNames = serializedCompanions.Split(',');
        ownedCompanions = new List<CompanionCard>();

        foreach (string cName in companionNames)
        {
            CompanionCard loadedCompanion = Resources.Load<CompanionCard>($"Cards/{cName}");
            if (loadedCompanion != null)
            {
                ownedCompanions.Add(loadedCompanion);
                Debug.Log($"Loaded companion: {loadedCompanion.CompanionName}");
            }
            else
            {
                Debug.LogError($"Failed to load companion from Resources: {cName}");
            }
        }

        Debug.Log($"Loaded {ownedCompanions.Count} companions from PlayerPrefs.");
    }


    public void SavePlayerInventoryToPrefs()
    {
        if (CardsInInventory != null && CardsInInventory.Length > 0)
        {
            string serializedInventory = string.Join(",", CardsInInventory);
            PlayerPrefs.SetString("PlayerInventory", serializedInventory);
        }
        else
        {
            PlayerPrefs.SetString("PlayerInventory", "");
        }
        PlayerPrefs.Save();
        Debug.Log("Player inventory saved to PlayerPrefs.");
    }

    // Modify your LoadGameState to load owned companions plus ensure spiders & mini-battle pool also load
    public void LoadGameState()
    {
        if (playerInstance != null)
        {
            playerInstance.transform.position = PlayerCoord;
            CurrentHealth = PlayerPrefs.GetInt("CurrentHealth", 100);
            playerInstance.Gold = CurrentCoins;
            playerInstance.Inventory = new List<string>(CardsInInventory);
            playerInstance.PlayerName = PlayerName;
            LoadOwnedCompanionsFromPrefs();
            LoadMiniBattleCardPoolFromPrefs();
            LoadSpiderStates();
            Debug.Log("Game state loaded.");
        }
        hasGameStateLoaded = true;
    }

    // Loads a new scene
    public void LoadScene(string nextScene, List<string> newObjectives = null, int startingObjectiveIndex = 0)
    {
        if (newObjectives != null)
        {
            ObjectiveManager objectiveManager = FindObjectOfType<ObjectiveManager>();
            if (objectiveManager != null)
            {
                objectiveManager.SetObjectives(newObjectives);
                objectiveManager.SetCurrentObjectiveIndex(startingObjectiveIndex);
            }
        }

        SavePlayerState();
        SceneManager.LoadScene(nextScene);
    }


    // Resets state
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
        defeatedSpiders.Clear();
        PlayerPrefs.DeleteKey("DefeatedSpiders");
        Debug.Log("Game state cleared.");
    }

    // Gives gold for mini battles
    public void RewardMiniBattleGold(int rewardAmount)
    {
        UpdatePlayerCoinCount(CurrentCoins1 + rewardAmount);
        Debug.Log($"Player rewarded with {rewardAmount} gold for mini-battle victory. Total coins: {CurrentCoins1}");
    }

    // Records mini battle win
    public void AddMiniBattleWin(string spiderID, int rewardAmount)
    {
        minibattleWins++;
        RewardMiniBattleGold(rewardAmount);
        MarkSpiderDefeated(spiderID);
        Debug.Log($"Mini-battle win recorded. Spider defeated: {spiderID}. Total wins: {minibattleWins}");
    }

    // Records mini battle loss
    public void AddMiniBattleLoss()
    {
        minibattleLosses++;
        Debug.Log($"Mini-battle loss recorded. Total losses: {minibattleLosses}");
    }

    // Saves spiders
    public void SaveSpiderStates()
    {
        string savedSpiders = string.Join(",", defeatedSpiders);
        PlayerPrefs.SetString("DefeatedSpiders", savedSpiders);
        PlayerPrefs.Save();
        Debug.Log("Spider defeat states saved.");
    }

    // Loads spiders
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

    // Gets all mini battle cards
    public List<Card> GetMiniBattleCardPool()
    {
        if (miniBattleCardPool == null)
        {
            miniBattleCardPool = new List<Card>();
            Debug.LogWarning("MiniBattleCardPool was null. Initialized with an empty list.");
        }
        return miniBattleCardPool;
    }

    // Updates all mini battle cards
    public void UpdateMiniBattleCardPool(List<Card> newPool)
    {
        miniBattleCardPool = newPool;
        SaveMiniBattleCardPoolToPrefs();
        Debug.Log("Mini-battle card pool updated and saved.");
    }

    // Ensures a default card
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

    // Saves card pool
    public void SaveMiniBattleCardPoolToPrefs()
    {
        List<string> cardNames = miniBattleCardPool.Select(card => card.Name).ToList();
        string serializedPool = string.Join(",", cardNames);
        PlayerPrefs.SetString("MiniBattleCardPool", serializedPool);
        PlayerPrefs.Save();
        Debug.Log("Mini-battle card pool saved to PlayerPrefs.");
    }

    // Loads card pool
    public void LoadMiniBattleCardPoolFromPrefs()
    {
        string serializedPool = PlayerPrefs.GetString("MiniBattleCardPool", "");
        if (!string.IsNullOrEmpty(serializedPool))
        {
            string[] cardNames = serializedPool.Split(',');
            miniBattleCardPool = cardNames.Select(cardName => Resources.Load<Card>($"Cards/{cardName}"))
                                          .Where(card => card != null).ToList();
            Debug.Log("Mini-battle card pool loaded from PlayerPrefs.");
        }
        else
        {
            Debug.LogWarning("No saved mini-battle card pool found in PlayerPrefs.");
        }
    }

    // Marks an objective
    public void CompleteObjective(string objectiveName)
    {
        // Implementation goes here
    }

    public List<string> GetObjectivesForScene(string sceneName)
    {
        if (sceneName == "Level1")
        {
            return new List<string> { "Talk to NPC3" };
        }
        // Add other levels here if needed
        return new List<string>();
    }


    // Adds a companion
    private CompanionCard selectedCompanion;
    public void AddCompanion(CompanionCard companion)
    {
        if (companion == null)
        {
            Debug.LogError("Attempted to add a null companion to GameManager.");
            return;
        }

        // Prevent duplicate companions
        if (ownedCompanions.Contains(companion))
        {
            Debug.LogWarning($"Companion {companion.CompanionName} is already in the owned list.");
            return;
        }

        ownedCompanions.Add(companion);
        SaveOwnedCompanionsToPrefs(); // Save to PlayerPrefs

        Debug.Log($"Companion added: {companion.CompanionName}. Total companions: {ownedCompanions.Count}");
    }

    public void ClearCompanion()
    {
        selectedCompanion = null;
        PlayerPrefs.SetInt("PlayerCompanionType", (int)CompanionType.None); // Set to None
        PlayerPrefs.Save();
        Debug.Log("Companion cleared from loadout and set to None.");
    }

    public CompanionType GetCompanionType()
    {
        int savedType = PlayerPrefs.GetInt("PlayerCompanionType", (int)CompanionType.None);
        Debug.Log($"Loaded companion type: {(CompanionType)savedType}");
        return (CompanionType)savedType;
    }


    // Sets companion type
    public void SetCompanionType(CompanionType companionType)
    {
        PlayerPrefs.SetInt("PlayerCompanionType", (int)companionType);
        PlayerPrefs.Save();
        Debug.Log($"Companion type saved: {companionType}");
    }



    // Runs each time a scene is loaded
    private void SceneLoadup(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        UpdateParticlesState();
        UpdatePostProcessingState();
        UpdateBrightnessState();
    }

    // Toggles particles
    private void UpdateParticlesState()
    {
        bool areParticlesOn = PlayerPrefs.GetInt("AreParticlesOn", 1) == 1;
        GameObject[] particleObjects = GameObject.FindGameObjectsWithTag("particle");
        foreach (GameObject particleObject in particleObjects) particleObject.SetActive(areParticlesOn);
        Debug.Log($"Particles are now {(areParticlesOn ? "enabled" : "disabled")}.");
    }

    // Toggles post-processing
    private void UpdatePostProcessingState()
    {
        bool areEffectsOn = PlayerPrefs.GetInt("AreEffectsOn", 1) == 1;
        GameObject[] effectsObjects = GameObject.FindGameObjectsWithTag("effects");
        if (effectsObjects.Length > 0)
        {
            foreach (GameObject effectsObject in effectsObjects) effectsObject.SetActive(areEffectsOn);
            Debug.Log($"Effects are now {(areEffectsOn ? "enabled" : "disabled")}.");
        }
        else
        {
            Debug.LogWarning("No objects with the tag 'effects' found in the scene.");
        }
    }

    // Adjusts brightness
    public static void UpdateBrightnessState()
    {
        float percentage = PlayerPrefs.GetFloat("lightBrightness");
        GameObject mainLightObject = GameObject.FindWithTag("SceneLight");
        if (mainLightObject != null)
        {
            Light mainLight = mainLightObject.GetComponent<Light>();
            if (mainLight != null)
            {
                float originalIntensity = mainLight.intensity;
                float adjustedIntensity = originalIntensity * (1 + percentage);
                mainLight.intensity = Mathf.Max(0, adjustedIntensity);
                Debug.Log($"Main light brightness adjusted by {percentage * 100}%. New intensity: {mainLight.intensity}");
            }
            else
            {
                Debug.LogWarning("The GameObject tagged 'MainLight' does not have a Light component.");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with the tag 'MainLight' was found in the scene.");
        }
    }
}