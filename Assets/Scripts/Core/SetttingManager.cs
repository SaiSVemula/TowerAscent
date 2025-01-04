using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    public Button[] BattleDifficultyModes;
    public Button[] ShadowToggler;
    public Button[] ParticleToggler;
    public Button[] EffectToggler;
    public Button[] CameraPOVModes;
    public Button[] MenuModeButtons;
    public Slider CameraMovementSensitivity;
    public Slider MasterVol;
    public Slider MusicVol;
    public Slider sfxVol;
    public Button WEBGLfullscreen;
    public Button SaveGameButton;
    private static SettingsManager instance;
    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;
    private LevelLoader levelLoader;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null) 
        { 
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }

        UpdateButtonInGroup(MenuModeButtons, 0);
        GroupUpButtons(BattleDifficultyModes, (index) => SetDifficulty(index));
        GroupUpButtons(ShadowToggler, (index) => SetShadows(index == 0));
        GroupUpButtons(ParticleToggler, (index) => SetParticles(index == 0));
        GroupUpButtons(EffectToggler, (index) => SetEffects(index == 0));
        GroupUpButtons(CameraPOVModes, (index) => SetViewMode(index == 0));
        GroupUpButtons(MenuModeButtons, (index) => SetPanel(index));

        WEBGLfullscreen.onClick.AddListener(ToggleFullscreen);
        SaveGameButton.onClick.AddListener(ToggleSaveGame);

        CameraMovementSensitivity.onValueChanged.AddListener(SetCameraSensitivity);
        MasterVol.onValueChanged.AddListener(SetMasterVolume);
        MusicVol.onValueChanged.AddListener(SetMusicVolume);
        sfxVol.onValueChanged.AddListener(SetSFXVolume);

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
        generalPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        panelToShow.SetActive(true);
    }

    public void exit() 
    {
        // Load the previous scene when exiting the settings menu
        string previousScene = GameManager.Instance.PreviousScene;

        if (string.IsNullOrEmpty(previousScene))
        {
            Debug.LogError("Previous scene not set!");
            return;
        }

        if (previousScene == "StartPage")
        {
            levelLoader.LoadScene("SettingsPage", "StartPage");
        }
        else if (previousScene == "Level 0" || previousScene == "Level 1" || previousScene == "Level 2")
        {
            GameManager.Instance.LoadPlayerState();
            LoadManager.TempLoadGameState();
        }
        else
        {
            Debug.LogWarning($"Unrecognized previous scene: {previousScene}. Defaulting to StartPage.");
            levelLoader.LoadScene("SettingsPage", "StartPage");
        }
    }


    private void GroupUpButtons(Button[] buttons, System.Action<int> onClickAction)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() =>
            {
                UpdateButtonInGroup(buttons, index);
                onClickAction(index);
            });
        }
    }

    private void UpdateButtonInGroup(Button[] buttons, int activeIndex)
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
        GameManager.Instance.GameDifficulty = (Difficulty)index;
        PlayerPrefs.SetInt("PlayersGameDifficulty", index);
        Debug.Log($"Difficulty set to: {index}");
    }

    private void SetShadows(bool shadowsOn){
        PlayerPrefs.SetInt("AreShadowsOn", shadowsOn ? 1 : 0);
        QualitySettings.shadows = shadowsOn ? ShadowQuality.All : ShadowQuality.Disable;
    }

    private void SetParticles(bool particlesOn){PlayerPrefs.SetInt("AreParticlesOn", particlesOn ? 1 : 0); }

    private void SetEffects(bool effectsOn){PlayerPrefs.SetInt("AreEffectsOn", effectsOn ? 1 : 0); }

    private void SetViewMode(bool IsInThirdPerson){PlayerPrefs.SetInt("IsInThirdPerson", IsInThirdPerson ? 1 : 0);}



    private void SetPanel(int index)
    {
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

    private void SetCameraSensitivity(float sensitivity){PlayerPrefs.SetFloat("MovementCamSensitivity", sensitivity);}
    private void SetMasterVolume(float volume){PlayerPrefs.SetFloat("SoundMasterVol", volume);}
    private void SetMusicVolume(float volume){PlayerPrefs.SetFloat("SoundMusicVol", volume);}
    private void SetSFXVolume(float volume){PlayerPrefs.SetFloat("SoundSFXVol", volume);}
    private void ToggleFullscreen(){Screen.fullScreen = !Screen.fullScreen;}
    private void ToggleSaveGame()
    {
        if (GameManager.Instance.PreviousScene == "Level 0" || 
            GameManager.Instance.PreviousScene == "Level 1" || 
            GameManager.Instance.PreviousScene == "Level 2")
        {
            // Retrieve the player's saved position from PlayerPrefs
            Vector3 currentPlayerLocation = new Vector3(
                PlayerPrefs.GetFloat("templocation_x", 0),
                PlayerPrefs.GetFloat("templocation_y", 0),
                PlayerPrefs.GetFloat("templocation_z", 0)
            );

            // Update the player's location in the GameManager
            GameManager.Instance.UpdatePlayerLocation(currentPlayerLocation);

            // Save the updated player state and game state
            GameManager.Instance.SavePlayerState();
            SaveGame.SaveGameState();
            
        }

    }


    private void RestoreSettings()
    {
        int tempdifficulty = PlayerPrefs.GetInt("PlayersGameDifficulty", 0);
        SetDifficulty(tempdifficulty);

        bool tempshadow = PlayerPrefs.GetInt("AreShadowsOn", 1) == 1;
        SetShadows(tempshadow);

        bool tempparticle = PlayerPrefs.GetInt("AreParticlesOn", 1) == 1;
        SetParticles(tempparticle);

        bool tempeffect = PlayerPrefs.GetInt("AreEffectsOn", 1) == 1;
        SetEffects(tempeffect);

        bool temppov = PlayerPrefs.GetInt("IsInThirdPerson", 1) == 1;
        SetViewMode(temppov);

        float tempcamsens = PlayerPrefs.GetFloat("MovementCamSensitivity", 0.1f);
        CameraMovementSensitivity.value = tempcamsens;
        SetCameraSensitivity(tempcamsens);

        float tempmastervol = PlayerPrefs.GetFloat("SoundMasterVol", 0.5f);
        MasterVol.value = tempmastervol;
        SetMasterVolume(tempmastervol);

        float tempmusicvol = PlayerPrefs.GetFloat("SoundMusicVol", 0.5f);
        MusicVol.value = tempmusicvol;
        SetMusicVolume(tempmusicvol);

        float tempsfxvol = PlayerPrefs.GetFloat("SoundSFXVol", 0.5f);
        sfxVol.value = tempsfxvol;
        SetSFXVolume(tempsfxvol);


        UpdateButtonInGroup(BattleDifficultyModes, tempdifficulty);
        UpdateButtonInGroup(ShadowToggler, tempshadow ? 0 : 1);
        UpdateButtonInGroup(ParticleToggler, tempparticle ? 0 : 1);
        UpdateButtonInGroup(EffectToggler, tempeffect ? 0 : 1);
        UpdateButtonInGroup(CameraPOVModes, temppov ? 0 : 1);
    }
}
