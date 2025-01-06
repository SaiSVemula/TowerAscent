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

