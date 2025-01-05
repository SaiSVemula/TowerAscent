using UnityEngine;

/*
This Is Quite A Simple Code That Is Used To Save The Gamemanager (All crucial game data) to PlayerPrefs 
For Persisitent Data Storage (i.e allows us to resume the game from the browser) 
*/
public class SaveManager : MonoBehaviour
{
    public static void SaveGameState()
    {
        var currentGameManager = GameManager.Instance; // Gets The GameManager Instance

        // Saves Everything in the gamemanager to PlayerPrefs For Persistent Data Storage
        PlayerPrefs.SetString("SavedScene", currentGameManager.GetCurrentScene());
        PlayerPrefs.SetFloat("PlayerCoordX", currentGameManager.GetPlayerLocation().x);
        PlayerPrefs.SetFloat("PlayerCoordY", currentGameManager.GetPlayerLocation().y);
        PlayerPrefs.SetFloat("PlayerCoordZ", currentGameManager.GetPlayerLocation().z);
        PlayerPrefs.SetInt("CurrentHealth", currentGameManager.GetPlayerHealth());
        PlayerPrefs.SetInt("CurrentCoins", currentGameManager.GetPlayerCoinCount());
        PlayerPrefs.SetString("PlayerName", currentGameManager.GetPlayerName());
        PlayerPrefs.SetInt("MinibattleWins", currentGameManager.GetMinibattleWins());
        PlayerPrefs.SetInt("MinibattleLosses", currentGameManager.GetMinibattleLosses());
        PlayerPrefs.SetInt("BigbattleWins", currentGameManager.GetBigbattleWins());
        PlayerPrefs.SetInt("BigbattleLosses", currentGameManager.GetBigbattleLosses());

        PlayerPrefs.Save();
    }
}
