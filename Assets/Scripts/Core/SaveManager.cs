//using UnityEngine;

///*
//This Is Quite A Simple Code That Is Used To Save The Gamemanager (All crucial game data) to PlayerPrefs 
//For Persisitent Data Storage (i.e allows us to resume the game from the browser) 
//*/
//public class SaveManager : MonoBehaviour
//{
//    // Modify the SaveGameState method to also save CardsInInventory, ownedCompanions, spiders, and miniBattleCardPool.
//    public static void SaveGameState()
//    {
//        var currentGameManager = GameManager.Instance;

//        // Existing saves
//        PlayerPrefs.SetString("SavedScene", currentGameManager.GetCurrentScene());
//        PlayerPrefs.SetFloat("PlayerCoordX", currentGameManager.GetPlayerLocation().x);
//        PlayerPrefs.SetFloat("PlayerCoordY", currentGameManager.GetPlayerLocation().y);
//        PlayerPrefs.SetFloat("PlayerCoordZ", currentGameManager.GetPlayerLocation().z);
//        PlayerPrefs.SetInt("CurrentHealth", currentGameManager.GetPlayerHealth());
//        PlayerPrefs.SetInt("CurrentCoins", currentGameManager.GetPlayerCoinCount());
//        PlayerPrefs.SetString("PlayerName", currentGameManager.GetPlayerName());
//        PlayerPrefs.SetInt("MinibattleWins", currentGameManager.GetMinibattleWins());
//        PlayerPrefs.SetInt("MinibattleLosses", currentGameManager.GetMinibattleLosses());
//        PlayerPrefs.SetInt("BigbattleWins", currentGameManager.GetBigbattleWins());
//        PlayerPrefs.SetInt("BigbattleLosses", currentGameManager.GetBigbattleLosses());

//        // New saves
//        currentGameManager.SavePlayerInventoryToPrefs();  // Saves CardsInInventory
//        currentGameManager.SaveOwnedCompanionsToPrefs();  // Saves ownedCompanions
//        currentGameManager.SaveSpiderStates();            // Saves defeated spiders
//        currentGameManager.SaveMiniBattleCardPoolToPrefs(); // Saves miniBattleCardPool

//        PlayerPrefs.Save();
//        Debug.Log("Full game state saved (including inventory, companions, spiders, and mini-battle pool).");
//    }
//}

using UnityEngine;
using System.Collections;

public static class SaveManager
{
    public static void SaveAllData()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("SaveAllData called, but GameManager.Instance is null!");
            return;
        }

        gm.UpdateCardsInInventory();

        PlayerPrefs.SetString("SavedScene", gm.GetCurrentScene());


        Debug.Log($"Saved Scene: {gm.GetCurrentScene()}");

        // Save transform coordinates
        Vector3 playerPos = gm.GetPlayerLocation();
        PlayerPrefs.SetFloat("PlayerCoordX", gm.GetPlayerLocation().x);
        PlayerPrefs.SetFloat("PlayerCoordY", gm.GetPlayerLocation().y);
        PlayerPrefs.SetFloat("PlayerCoordZ", gm.GetPlayerLocation().z);

        // Save key player stats
        PlayerPrefs.SetInt("CurrentHealth", gm.GetPlayerHealth());
        PlayerPrefs.SetInt("CurrentCoins", gm.GetPlayerCoinCount());
        PlayerPrefs.SetString("PlayerName", gm.GetPlayerName());

        // Save battle stats
        PlayerPrefs.SetInt("MinibattleWins", gm.GetMinibattleWins());
        PlayerPrefs.SetInt("MinibattleLosses", gm.GetMinibattleLosses());
        PlayerPrefs.SetInt("BigbattleWins", gm.GetBigbattleWins());
        PlayerPrefs.SetInt("BigbattleLosses", gm.GetBigbattleLosses());

        // Save arrays & lists using existing GameManager utility methods
        gm.SavePlayerInventoryToPrefs();
        gm.SaveOwnedCompanionsToPrefs();
        gm.SaveSpiderStates();
        gm.SaveMiniBattleCardPoolToPrefs();

        PlayerPrefs.Save();
        Debug.Log("SaveAllData: Full game state saved to PlayerPrefs.");
    }
}

