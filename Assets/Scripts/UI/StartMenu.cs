using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.Instance.Clear();
        SceneManager.LoadScene("ExplorationScene");
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
        GameManager.Instance.UpdateCurrentScene();
        SceneManager.LoadScene("SettingsPage");
    }

    public void Quit()
    {
        SceneManager.LoadScene("EndPage");
    }
}
