using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    [Header("Settings Panels")]
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;

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
      GameManager.Instance.LoadScene();  
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
