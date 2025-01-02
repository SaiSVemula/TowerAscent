using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    public static void LoadGameState()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            // Load the saved scene
            string savedScene = PlayerPrefs.GetString("SavedScene");
            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            levelLoader.LoadScene("StartPage", savedScene);


            // Start a coroutine to wait until the scene is fully loaded before adjusting the player's position
            GameManager.Instance.StartCoroutine(WaitForSceneLoad());
        }
        else
        {
            LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
            levelLoader.LoadScene("StartPage", "StartPage");
            Debug.LogWarning("No saved scene found in PlayerPrefs.");
        }
    }

    private static IEnumerator WaitForSceneLoad()
    {
        // Wait until the scene is loaded and the player object is available
        yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null);

        // Find the player object by tag
        Transform playerTransform = GameObject.FindWithTag("Player").transform;

        // Retrieve the saved coordinates from PlayerPrefs
        float playerCoordX = PlayerPrefs.GetFloat("PlayerCoordX", 0);
        float playerCoordY = PlayerPrefs.GetFloat("PlayerCoordY", 0);
        float playerCoordZ = PlayerPrefs.GetFloat("PlayerCoordZ", 0);

        // Set the player's position to the saved coordinates
        playerTransform.position = new Vector3(playerCoordX, playerCoordY, playerCoordZ);

        // Log the loaded position
        Debug.Log($"Player position loaded: {playerTransform.position}");
    }
}
