using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;

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


    public void GeneralSettings()
    {
        showpanel(generalPanel);
    }

    public void AudioSettings()
    {
        showpanel(audioPanel);
    }

    public void VideoSettings()
    {
        showpanel(videoPanel);
    }

    public void exit()
    {
        // Load the previous scene when exiting the settings menu
        if (!string.IsNullOrEmpty(GameManager.Instance.PreviousScene))
        {
            //GameManager.Instance.LoadScene(GameManager.Instance.PreviousScene);
            levelLoader.LoadScene("SettingsPage", GameManager.Instance.PreviousScene);
        }
        else
        {
            Debug.LogError("Previous scene not set!");
        }
    }

    private void showpanel(GameObject panelToShow)
    {
        // Deactivate all panels
        generalPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);

        // Activate the settings panel and the selected panel
        panelToShow.SetActive(true);
    }
}
