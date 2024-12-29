using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    private LevelLoader levelLoader;
    private void Start()
    {
        // Find the LevelLoader instance in the current scene
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
    }

    public void NewGame()
    {
        if (levelLoader != null)
        {
            GameManager.Instance.Clear();
            levelLoader.LoadScene("StartMenu", "StartCutScene"); // Pass current and next scenes
        }
    }

    public void Resume()
    {
        if (PlayerPrefs.HasKey("SavedScene")) // Check if a saved game exists
        {
            LoadManager.LoadGameState(); // Load the saved game state from PlayerPrefs
        }
        else
        {
            Debug.LogWarning("No saved game found!");
        }
    }

    public void Settings()
    {
        if (levelLoader != null)
        {
            GameManager.Instance.UpdateCurrentScene();
            levelLoader.LoadScene("StartMenu", "SettingsPage"); // Transition to SettingsPage
        }
    }

    public void Quit()
    {
        if (levelLoader != null)
        {
            levelLoader.LoadScene("StartMenu", "EndPage"); // Transition to EndPage
        }
    }
}
