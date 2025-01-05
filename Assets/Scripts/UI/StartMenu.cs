using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
This code will handle the start page, each button is mapped to a method and we restore any states too here
*/
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

    private void NewGame() // method for starting a new game
    {
        AudioManager.instance.PlaySFX(0);
        if (levelLoader != null)
        {
            GameManager.Instance.Clear(); // make sure game manager is cleared

            // stores setting in temp variables
            int tempGameDifficulty = PlayerPrefs.GetInt("PlayersGameDifficulty", 1);
            int tempShadowsOn = PlayerPrefs.GetInt("AreShadowsOn", 1);
            int tempParticlesOn = PlayerPrefs.GetInt("AreParticlesOn", 1);
            int tempEffectsOn = PlayerPrefs.GetInt("AreEffectsOn", 1);
            int tempIsInThirdPerson = PlayerPrefs.GetInt("IsInThirdPerson", 1);
            float tempCameraSensitivity = PlayerPrefs.GetFloat("MovementCamSensitivity", 0.1f);
            float tempMasterVolume = PlayerPrefs.GetFloat("SoundMasterVol", 0.5f);
            float tempMusicVolume = PlayerPrefs.GetFloat("SoundMusicVol", 0.5f);
            float tempSFXVolume = PlayerPrefs.GetFloat("SoundSFXVol", 0.5f);
            float tempbrightness = PlayerPrefs.GetFloat("lightBrightness", 0.5f);

            // Clear PlayerPrefs (persistent storage)
            PlayerPrefs.DeleteAll();

            // restore users settins (so we have a new game but reatin settings)
            PlayerPrefs.SetInt("PlayersGameDifficulty", tempGameDifficulty);
            PlayerPrefs.SetInt("AreShadowsOn", tempShadowsOn);
            PlayerPrefs.SetInt("AreParticlesOn", tempParticlesOn);
            PlayerPrefs.SetInt("AreEffectsOn", tempEffectsOn);
            PlayerPrefs.SetInt("IsInThirdPerson", tempIsInThirdPerson);
            PlayerPrefs.SetFloat("MovementCamSensitivity", tempCameraSensitivity);
            PlayerPrefs.SetFloat("SoundMasterVol", tempMasterVolume);
            PlayerPrefs.SetFloat("SoundMusicVol", tempMusicVolume);
            PlayerPrefs.SetFloat("SoundSFXVol", tempSFXVolume);
            PlayerPrefs.SetFloat("lightBrightness", tempbrightness);
            PlayerPrefs.Save();

            // Load the start scene
            levelLoader.LoadScene("StartPage", "StartCutScene");
        }
    }


    public void Resume() // handles resuming
    {
        AudioManager.instance.PlaySFX(0);
        if (PlayerPrefs.HasKey("SavedScene")) { LoadManager.LoadGameState(); } // reinstate player and game stats and load the game (checks if we have a save file)
        else { Debug.LogWarning("No saved game found!"); }
    }

    public void Settings() // opens settings page
    {
        AudioManager.instance.PlaySFX(0);
        if (levelLoader != null)
        {
            GameManager.Instance.UpdateCurrentScene(); // updates scene so we know where to come back to when exiting
            levelLoader.LoadScene("StartPage", "SettingsPage"); // Transition to SettingsPage
        }
    }

    public void Quit() { 
        
        AudioManager.instance.PlaySFX(0);
        if (levelLoader != null) { levelLoader.LoadScene("StartPage", "EndPage"); } 
    
    } // Transition to EndPage where credits are shown
}
