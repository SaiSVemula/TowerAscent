using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static void SaveGameState()
    {
        var gm = GameManager.Instance;
        Debug.Log($"Saving game state... {gm.GetPlayerHealth()}");
        PlayerPrefs.SetString("SavedScene", gm.GetCurrentScene());
        PlayerPrefs.SetFloat("PlayerCoordX", gm.GetPlayerLocation().x);
        PlayerPrefs.SetFloat("PlayerCoordY", gm.GetPlayerLocation().y);
        PlayerPrefs.SetFloat("PlayerCoordZ", gm.GetPlayerLocation().z);
        PlayerPrefs.SetInt("CurrentHealth", gm.GetPlayerHealth());
        PlayerPrefs.SetInt("CurrentCoins", gm.GetPlayerCoinCount());
        PlayerPrefs.SetString("PlayerName", gm.GetPlayerName());
        
        
        PlayerPrefs.SetInt("MinibattleWins", gm.GetMinibattleWins());
        PlayerPrefs.SetInt("MinibattleLosses", gm.GetMinibattleLosses());
        PlayerPrefs.SetInt("BigbattleWins", gm.GetBigbattleWins());
        PlayerPrefs.SetInt("BigbattleLosses", gm.GetBigbattleLosses());


        PlayerPrefs.Save();

        int health = PlayerPrefs.GetInt("CurrentHealth");
        Debug.Log($"Player health saved: {health}");
        Debug.Log("Game state saved.");
    }
}
