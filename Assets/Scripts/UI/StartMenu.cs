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

    private void NewGame()
    {
        if (levelLoader != null)
        {
            string previousScene = GameManager.Instance.PreviousScene;
            Debug.Log($"Previous Scene: {previousScene}");

            GameManager.Instance.Clear();
            switch (previousScene)
            {
                case "SettingsPage":
                    // If we were in the SettingsPage, save current PlayerPrefs
                    int tempGameDifficulty = PlayerPrefs.GetInt("PlayersGameDifficulty", 1);
                    int tempShadowsOn = PlayerPrefs.GetInt("AreShadowsOn", 1);
                    int tempParticlesOn = PlayerPrefs.GetInt("AreParticlesOn", 1);
                    int tempEffectsOn = PlayerPrefs.GetInt("AreEffectsOn", 1);
                    int tempIsInThirdPerson = PlayerPrefs.GetInt("IsInThirdPerson", 1);
                    float tempCameraSensitivity = PlayerPrefs.GetFloat("MovementCamSensitivity", 0.1f);
                    float tempMasterVolume = PlayerPrefs.GetFloat("SoundMasterVol", 0.5f);
                    float tempMusicVolume = PlayerPrefs.GetFloat("SoundMusicVol", 0.5f);
                    float tempSFXVolume = PlayerPrefs.GetFloat("SoundSFXVol", 0.5f);

                    // Clear PlayerPrefs
                    PlayerPrefs.DeleteAll();

                    // Load default PlayerPrefs values
                    PlayerPrefs.SetInt("PlayersGameDifficulty", 1);
                    PlayerPrefs.SetInt("AreShadowsOn", 1);
                    PlayerPrefs.SetInt("AreParticlesOn", 1);
                    PlayerPrefs.SetInt("AreEffectsOn", 1);
                    PlayerPrefs.SetInt("IsInThirdPerson", 1);
                    PlayerPrefs.SetFloat("MovementCamSensitivity", 0.1f);
                    PlayerPrefs.SetFloat("SoundMasterVol", 0.5f);
                    PlayerPrefs.SetFloat("SoundMusicVol", 0.5f);
                    PlayerPrefs.SetFloat("SoundSFXVol", 0.5f);
                    PlayerPrefs.Save();

                    // After PlayerPrefs cleared, restore the previous settings
                    PlayerPrefs.SetInt("PlayersGameDifficulty", tempGameDifficulty);
                    PlayerPrefs.SetInt("AreShadowsOn", tempShadowsOn);
                    PlayerPrefs.SetInt("AreParticlesOn", tempParticlesOn);
                    PlayerPrefs.SetInt("AreEffectsOn", tempEffectsOn);
                    PlayerPrefs.SetInt("IsInThirdPerson", tempIsInThirdPerson);
                    PlayerPrefs.SetFloat("MovementCamSensitivity", tempCameraSensitivity);
                    PlayerPrefs.SetFloat("SoundMasterVol", tempMasterVolume);
                    PlayerPrefs.SetFloat("SoundMusicVol", tempMusicVolume);
                    PlayerPrefs.SetFloat("SoundSFXVol", tempSFXVolume);
                    PlayerPrefs.Save();

                    break;

                default:

                    PlayerPrefs.DeleteAll();
                    // If not in SettingsPage, load default values and start the game
                    PlayerPrefs.SetInt("PlayersGameDifficulty", 1);
                    PlayerPrefs.SetInt("AreShadowsOn", 1);
                    PlayerPrefs.SetInt("AreParticlesOn", 1);
                    PlayerPrefs.SetInt("AreEffectsOn", 1);
                    PlayerPrefs.SetInt("IsInThirdPerson", 1);
                    PlayerPrefs.SetFloat("MovementCamSensitivity", 0.1f);
                    PlayerPrefs.SetFloat("SoundMasterVol", 0.5f);
                    PlayerPrefs.SetFloat("SoundMusicVol", 0.5f);
                    PlayerPrefs.SetFloat("SoundSFXVol", 0.5f);
                    PlayerPrefs.Save();

                    break;
            }

            // Load the start scene
            levelLoader.LoadScene("StartPage", "StartCutScene");
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
            levelLoader.LoadScene("StartPage", "SettingsPage"); // Transition to SettingsPage
        }
    }

    public void Quit()
    {
        if (levelLoader != null)
        {
            levelLoader.LoadScene("StartPage", "EndPage"); // Transition to EndPage
        }
    }
}
