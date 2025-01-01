using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{

    [Header("Settings Panels")]
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;


    public Button[] difficultyButtons;
    public Button[] shadowButtons;
    public Button[] particleButtons;
    public Button[] effectButtons;
    public Button[] viewModeButtons; // 3rd Person / 1st Person buttons
    public Button[] panelButtons; // GeneralPanel, AudioPanel, VideoPanel buttons

    public Slider cameraSensitivitySlider;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public Button fullscreenButton;

    private static SettingsManager instance;

    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;

    private void Awake()
    {
        // Ensure that there is only one SettingsManager in the scene
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy the duplicate if there's already one in the scene
        }
    }

    private void Start()
    {
        AssignButtonGroup(difficultyButtons, (index) => SetDifficulty(index));
        AssignButtonGroup(shadowButtons, (index) => SetShadows(index == 0));
        AssignButtonGroup(particleButtons, (index) => SetParticles(index == 0));
        AssignButtonGroup(effectButtons, (index) => SetEffects(index == 0));
        AssignButtonGroup(viewModeButtons, (index) => SetViewMode(index == 0));
        AssignButtonGroup(panelButtons, (index) => SetPanel(index));

        fullscreenButton.onClick.AddListener(ToggleFullscreen);

        cameraSensitivitySlider.onValueChanged.AddListener(SetCameraSensitivity);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        RestoreSettings();
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

    private void showpanel(GameObject panelToShow)
    {
        // Deactivate all panels
        generalPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);

        // Activate the settings panel and the selected panel
        panelToShow.SetActive(true);
    }

    public void exit()
    {
        // Load the previous scene when exiting the settings menu
        if (!string.IsNullOrEmpty(GameManager.Instance.PreviousScene))
        {
            GameManager.Instance.LoadScene(GameManager.Instance.PreviousScene);
        }
        else
        {
            Debug.LogError("Previous scene not set!");
        }
    }

    private void AssignButtonGroup(Button[] buttons, System.Action<int> onClickAction)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonGroup(buttons, index);
                onClickAction(index);
            });
        }
    }

    private void UpdateButtonGroup(Button[] buttons, int activeIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == activeIndex)
            {
                buttons[i].interactable = false;
                buttons[i].GetComponent<Image>().color = selectedColor; 
            }
            else
            {
                buttons[i].interactable = true;
                buttons[i].GetComponent<Image>().color = unselectedColor; 
            }
        }
    }

    private void SetDifficulty(int index) // NOT DONE (SANDEEP)
    {
        PlayerPrefs.SetInt("SelectedDifficulty", index);
        Debug.Log($"Difficulty set to: {index}");
    }

    private void SetShadows(bool shadowsOn) //  DONE
    {
        PlayerPrefs.SetInt("ShadowsEnabled", shadowsOn ? 1 : 0);
        QualitySettings.shadows = shadowsOn ? ShadowQuality.All : ShadowQuality.Disable;
        Debug.Log($"Shadows set to: {shadowsOn}");
    }

    private void SetParticles(bool particlesOn) // NOT DONE
    {
        PlayerPrefs.SetInt("ParticlesEnabled", particlesOn ? 1 : 0); 
        Debug.Log($"Particles set to: {particlesOn}");
    }

    private void SetEffects(bool effectsOn) // NOT DONE
    {
        PlayerPrefs.SetInt("EffectsEnabled", effectsOn ? 1 : 0); 
        Debug.Log($"Effects set to: {effectsOn}");
    }

    private void SetViewMode(bool isThirdPerson) // DONE
    {
        PlayerPrefs.SetInt("IsThirdPerson", isThirdPerson ? 1 : 0);
        GameManager.Instance.UpdatePOV(isThirdPerson); // Pass the bool directly
        Debug.Log($"View mode set to: {(isThirdPerson ? "3rd Person" : "1st Person")}");
    }



    private void SetPanel(int index)
    {
        PlayerPrefs.SetInt("SelectedPanel", index); 
        switch (index)
        {
            case 0:
                Debug.Log("Panel set to: General Panel");
                break;
            case 1:
                Debug.Log("Panel set to: Audio Panel");
                break;
            case 2:
                Debug.Log("Panel set to: Video Panel");
                break;
            default:
                Debug.Log("Panel set to: Unknown");
                break;
        }
    }

    private void SetCameraSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("CameraSensitivity", sensitivity);
        Debug.Log($"Camera sensitivity set to: {sensitivity}");
    }

    private void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        Debug.Log($"Master volume set to: {volume}");
    }

    private void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        Debug.Log($"Music volume set to: {volume}");
    }

    private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        Debug.Log($"SFX volume set to: {volume}");
    }

    private void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        PlayerPrefs.SetInt("Fullscreen", Screen.fullScreen ? 1 : 0);
        Debug.Log($"Fullscreen toggled to: {Screen.fullScreen}");
    }

    private void RestoreSettings()
    {
        int savedDifficulty = PlayerPrefs.GetInt("SelectedDifficulty", 0);
        SetDifficulty(savedDifficulty);

        bool savedShadows = PlayerPrefs.GetInt("ShadowsEnabled", 1) == 1;
        SetShadows(savedShadows);

        bool savedParticles = PlayerPrefs.GetInt("ParticlesEnabled", 1) == 1;
        SetParticles(savedParticles);

        bool savedEffects = PlayerPrefs.GetInt("EffectsEnabled", 1) == 1;
        SetEffects(savedEffects);

        bool savedViewMode = PlayerPrefs.GetInt("IsThirdPerson", 1) == 1;
        SetViewMode(savedViewMode);

        int savedPanel = PlayerPrefs.GetInt("SelectedPanel", 0);
        SetPanel(savedPanel);

        float savedCameraSensitivity = PlayerPrefs.GetFloat("CameraSensitivity", 1f);
        cameraSensitivitySlider.value = savedCameraSensitivity;
        SetCameraSensitivity(savedCameraSensitivity);

        float savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterVolumeSlider.value = savedMasterVolume;
        SetMasterVolume(savedMasterVolume);

        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicVolumeSlider.value = savedMusicVolume;
        SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sfxVolumeSlider.value = savedSFXVolume;
        SetSFXVolume(savedSFXVolume);

        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 0) == 1;
        Screen.fullScreen = savedFullscreen;
        Debug.Log($"Fullscreen restored: {Screen.fullScreen}");

        UpdateButtonGroup(difficultyButtons, savedDifficulty);

        UpdateButtonGroup(shadowButtons, savedShadows ? 0 : 1);
        UpdateButtonGroup(particleButtons, savedParticles ? 0 : 1);
        UpdateButtonGroup(effectButtons, savedEffects ? 0 : 1);
        UpdateButtonGroup(viewModeButtons, savedViewMode ? 0 : 1);
        UpdateButtonGroup(panelButtons, savedPanel);
    }
}
