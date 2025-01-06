using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/*
This Code Is Used To Load Up When Resuming A Game, Or When You Are Coming Back From The Settings Page
First 2 Methods: Full Reload
Last 2 Methods: Partial Reload (For When We Are Going From Settings Back To level, gamemanager still is tracking)
*/
public class LoadManager : MonoBehaviour
{
    //public static void LoadGameState()
    //{
    //    if (PlayerPrefs.HasKey("SavedScene")) // Ensure We Have A Save File (PlayerPref)
    //    {
    //        string sceneToLoad = PlayerPrefs.GetString("SavedScene"); // Get The Scene Saved In PlayerPref
    //        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
    //        levelLoader.LoadScene("StartPage", sceneToLoad); // Transition To The Scene That We Was On

    //        // Tell GameManager To Fetch And Update Itself
    //        GameManager.Instance.LoadGameState();

    //        // Co Routine To Ensure Scene Is Loaded
    //        GameManager.Instance.StartCoroutine(WaitForPlayerInScene());
    //    }
    //    else // Throws User Back To StartPage Again
    //    {
    //        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
    //        levelLoader.LoadScene("StartPage", "StartPage");
    //        Debug.LogWarning("No saved scene found in PlayerPrefs.");
    //    }
    //}

    //private static IEnumerator WaitForPlayerInScene() // Co routine that waits for scene to load and then Updates GameManager and Player
    //{
    //    yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null); // Wait Until The Player Object Is Found (So we Can Move It)

    //    Transform CurrentPlayerLocation = GameObject.FindWithTag("Player").transform;
    //    GameManager gameManager = GameManager.Instance;

    //    // Gets The Previous Location From The PlayerPref Abd Apply it to the Player
    //    float SavedXCoord = PlayerPrefs.GetFloat("PlayerCoordX", 0);
    //    float SavedYCoord = PlayerPrefs.GetFloat("PlayerCoordY", 0); // Defaults to 0
    //    float SavedZCoord = PlayerPrefs.GetFloat("PlayerCoordZ", 0);
    //    CurrentPlayerLocation.position = new Vector3(SavedXCoord, SavedYCoord, SavedZCoord);

    //    // Get the Fetched Inventory (Has To Be Saved as As String) And Split Up Into A Array
    //    string fetchedInventory = PlayerPrefs.GetString("PlayerInventory", "");
    //    string[] fetchedInventoryArray = string.IsNullOrEmpty(fetchedInventory) ? new string[0] : fetchedInventory.Split(',');
    //    gameManager.UpdatePlayerCards(fetchedInventoryArray);

    //    // Restores The Rest Of The Items Directy
    //    gameManager.UpdatePlayerHealth(PlayerPrefs.GetInt("CurrentHealth", 100));
    //    gameManager.UpdatePlayerCoinCount(PlayerPrefs.GetInt("CurrentCoins", 0));
    //    gameManager.UpdatePlayerName(PlayerPrefs.GetString("PlayerName", "Hero"));
    //    gameManager.UpdateMinibattleWins(PlayerPrefs.GetInt("MinibattleWins", 0));
    //    gameManager.UpdateMinibattleLosses(PlayerPrefs.GetInt("MinibattleLosses", 0));
    //    gameManager.UpdateBigbattleWins(PlayerPrefs.GetInt("BigbattleWins", 0));
    //    gameManager.UpdateBigbattleLosses(PlayerPrefs.GetInt("BigbattleLosses", 0));
    //}

    //private static Vector3 PlayerPosition;
    public static void TempLoadGameState()
    {
        string sceneToLoad = GameManager.Instance.PreviousScene; // Get The Scene Saved In PlayerPref
        LevelLoader levelLoader = Object.FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader instance not found!");
            return;
        }
        levelLoader.LoadScene("SettingsPage", sceneToLoad); // Transition To The Scene We Was On Before

        GameManager.Instance.StartCoroutine(TempWaitForPlayerInScene()); // CoRoutine Used to wait for player to be found
    }

    private static IEnumerator TempWaitForPlayerInScene()
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null); // Wait until the player is found in the scene so we can transform

        Transform CurrentPlayerLocation = GameObject.FindWithTag("Player").transform; // Current Players location

        // Getting The Coords from the PlayerPrefs
        float savedXCoord = PlayerPrefs.GetFloat("templocation_x", 0);
        float savedYCoord = PlayerPrefs.GetFloat("templocation_y", 0);
        float savedZCoord = PlayerPrefs.GetFloat("templocation_z", 0);

        // Transform the players Location
        Vector3 savedPlayerLocation = new Vector3(savedXCoord, savedYCoord, savedZCoord);
        CurrentPlayerLocation.position = savedPlayerLocation;

    }

    public static void LoadAllData()
    {
        if (!PlayerPrefs.HasKey("SavedScene"))
        {
            Debug.LogWarning("LoadAllData: No saved scene found in PlayerPrefs (no save data).");
            return;
        }


        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("LoadAllData called, but GameManager.Instance is null!");
            return;
        }

        //Read transform coordinates
        GameManager.Instance.StartCoroutine(WaitForPlayerInScene());

        // Read key player stats
        int health = PlayerPrefs.GetInt("CurrentHealth", 100);
        gm.UpdatePlayerHealth(health);

        int coins = PlayerPrefs.GetInt("CurrentCoins", 0);
        gm.UpdatePlayerCoinCount(coins);

        string playerName = PlayerPrefs.GetString("PlayerName", "Hero");
        gm.UpdatePlayerName(playerName);

        // Read battle stats
        int miniWins = PlayerPrefs.GetInt("MinibattleWins", 0);
        int miniLoss = PlayerPrefs.GetInt("MinibattleLosses", 0);
        int bigWins = PlayerPrefs.GetInt("BigbattleWins", 0);
        int bigLoss = PlayerPrefs.GetInt("BigbattleLosses", 0);

        gm.UpdateMinibattleWins(miniWins);
        gm.UpdateMinibattleLosses(miniLoss);
        gm.UpdateBigbattleWins(bigWins);
        gm.UpdateBigbattleLosses(bigLoss);

        // Use GameManager's existing methods to load the rest
        gm.LoadOwnedCompanionsFromPrefs();
        gm.LoadMiniBattleCardPoolFromPrefs();
        gm.LoadSpiderStates();
        
        gm.LoadPlayerInventoryFromPrefs();

        gm.LoadGameState();

        string sceneToLoad = PlayerPrefs.GetString("SavedScene", ""); // Get The Scene Saved In PlayerPref
        SceneManager.LoadScene(sceneToLoad);
        //GameManager.Instance.UpdateSavedScene(sceneToLoad);
        Debug.Log("LoadAllData: Full game state loaded from PlayerPrefs and assigned to GameManager.");
    }

    private static IEnumerator WaitForPlayerInScene() // Co routine that waits for scene to load and then Updates GameManager and Player
    {
        yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null); // Wait Until The Player Object Is Found (So we Can Move It)

        Transform CurrentPlayerLocation = GameObject.FindWithTag("Player").transform;
        GameManager gameManager = GameManager.Instance;

        // Gets The Previous Location From The PlayerPref Abd Apply it to the Player
        float SavedXCoord = PlayerPrefs.GetFloat("PlayerCoordX", 0);
        float SavedYCoord = PlayerPrefs.GetFloat("PlayerCoordY", 0); // Defaults to 0
        float SavedZCoord = PlayerPrefs.GetFloat("PlayerCoordZ", 0);
        CurrentPlayerLocation.position = new Vector3(SavedXCoord, SavedYCoord, SavedZCoord);
    }
}