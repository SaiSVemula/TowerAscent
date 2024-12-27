using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;

    private string previousScene; // Store the scene that called the settings menu

    private void Start()
    {
        // Save the current scene name when the settings menu is loaded
        previousScene = GameManager.Instance.GetCurrentScene();
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
        if (!string.IsNullOrEmpty(previousScene))
        {
            GameManager.Instance.LoadScene(previousScene);
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
