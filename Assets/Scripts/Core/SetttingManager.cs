using UnityEngine;
using UnityEngine.UI;
/*This Script Is Used To COntrol The Settings Page, It Will Save Settings,
 Manage Settings And Retrieve Settings From PlayerPrefs*/
public class SettingsManager : MonoBehaviour
{
    // Refrence To Panels For Submenus
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    // Refrence To Array Of Buttons To Make Toggles (As Unity Toggles Do Not Work Well)
    public Button[] BattleDifficultyModes;
    public Button[] ShadowToggler;
    public Button[] ParticleToggler;
    public Button[] EffectToggler;
    public Button[] CameraPOVModes;
    public Button[] MenuModeButtons;

    // Refrence To Sliders In The Code
    public Slider CameraMovementSensitivity;
    public Slider MasterVol;
    public Slider MusicVol;
    public Slider sfxVol;
    public Slider brightness;
    // Refrence To Bare Buttons
    public Button WEBGLfullscreen;
    public Button SaveGameButton;

    private static SettingsManager instance;
    public Color selectedColor = Color.green;
    public Color unselectedColor = Color.white;
    private LevelLoader levelLoader;

    private void Awake() // Makes Sure That We only Have One Settings Manager
    {
        if (instance == null) { instance = this; } 
        else { Destroy(gameObject); }
    }

    private void Start() // Gets LevelLoader, Makes Button Groups And Sets Listeners To Excecure Respective Setting Methods
    {

        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null) 
        { 
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }

        UpdateButtonInGroup(MenuModeButtons, 0); // This is a simple way to ensure we start on the first panel (general)

        // We Make our button groups here and link them to setter methods
        GroupUpButtons(BattleDifficultyModes, (index) => SetDifficulty(index)); // Arrow notation used for Streamlining code (would need extra method per call)
        GroupUpButtons(ShadowToggler, (index) => SetShadows(index == 0));
        GroupUpButtons(ParticleToggler, (index) => SetParticles(index == 0));
        GroupUpButtons(EffectToggler, (index) => SetEffects(index == 0));
        GroupUpButtons(CameraPOVModes, (index) => SetViewMode(index == 0));
        GroupUpButtons(MenuModeButtons, (index) => SetPanel(index));

        // Listeners for bare buttons to call a setter
        WEBGLfullscreen.onClick.AddListener(ToggleFullscreen);
        SaveGameButton.onClick.AddListener(ToggleSaveGame);

        // Slider Listeners which check for a value change and sets in methods
        CameraMovementSensitivity.onValueChanged.AddListener(SetCameraSensitivity);
        MasterVol.onValueChanged.AddListener(SetMasterVolume);
        MusicVol.onValueChanged.AddListener(SetMusicVolume);
        sfxVol.onValueChanged.AddListener(SetSFXVolume);
        brightness.onValueChanged.AddListener(SetBrightness);

        // every time we start up the scene it will restore the users settings visually
        getfromPlayerPref();
    }

    // These Methods simply are linked to menu buttons to turn sub panels on and off
    public void GeneralSettings() { showpanel(generalPanel); }
    public void AudioSettings() { showpanel(audioPanel); }
    public void VideoSettings() { showpanel(videoPanel); }

    private void showpanel(GameObject panelToShow) // this method enables the panel you click the corresponding button on and disables the other 2
    {
        generalPanel.SetActive(false);
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        panelToShow.SetActive(true);
    }

    public void exit()  // handles the logic when you click the exit button
    {
        string previousScene = GameManager.Instance.PreviousScene; // checks what the previous scene you was on is 

        if (string.IsNullOrEmpty(previousScene)) { return; } // exit method if no scene found

        if (previousScene == "StartPage") { levelLoader.LoadScene("SettingsPage", "StartPage"); } // go back to start if thats the last page

        else if (previousScene == "Level 0" || previousScene == "Level 1" || previousScene == "Level 2") // if we was on a level then load up previous state in LoadManager
        {
            GameManager.Instance.LoadPlayerState();
            LoadManager.TempLoadGameState();
        }
        else { levelLoader.LoadScene("SettingsPage", "StartPage"); } // just a failsafe if for some reason we was on none of those scenes default back to start
    }


    private void GroupUpButtons(Button[] buttongroup, System.Action<int> onClickAction) // This method will create our button groups
    {
        for (int i = 0; i < buttongroup.Length; i++) // loop through buttons
        {
            int currentbutton = i;
            // adds a listener to the button and updates its color and functional state
            buttongroup[i].onClick.AddListener(() =>
            {
                UpdateButtonInGroup(buttongroup, currentbutton);
                onClickAction(currentbutton);
            });
        }
    }

    private void UpdateButtonInGroup(Button[] buttongroup, int activebutton) // functionally and visually updates buttons in a group
    {
        for (int i = 0; i < buttongroup.Length; i++) // loop through buttons
        {
            if (i == activebutton) // checks if the current button  iteration is the one that is clicked / active
            {
                buttongroup[i].interactable = false; // stop interaction 
                buttongroup[i].GetComponent<Image>().color = selectedColor; // change color (done in script as using unitys inspector seems to be overridden)
            }
            else
            {
                buttongroup[i].interactable = true;
                buttongroup[i].GetComponent<Image>().color = unselectedColor; 
            }
        }
    }

    private void SetPanel(int index) // seems uneeded but is used for color assigment
    {
        switch (index)
        {
            case 0: break;
            case 1: break;
            case 2: break;
            default: break;
        }
    }

    // Setters For the Difficulty (Done different via gamemanager too)
    private void SetDifficulty(int buttoninthegroup)
    {
        GameManager.Instance.GameDifficulty = (Difficulty)buttoninthegroup;
        PlayerPrefs.SetInt("PlayersGameDifficulty", buttoninthegroup);
    }

    // Sets Shadows Globally 
    private void SetShadows(bool shadowsOn){
        PlayerPrefs.SetInt("AreShadowsOn", shadowsOn ? 1 : 0);
        QualitySettings.shadows = shadowsOn ? ShadowQuality.All : ShadowQuality.Disable;
    }

    // Direct PlayPref Setters Which Are Refrenced in other scripts
    private void SetParticles(bool particlesOn){PlayerPrefs.SetInt("AreParticlesOn", particlesOn ? 1 : 0); }
    private void SetEffects(bool effectsOn){PlayerPrefs.SetInt("AreEffectsOn", effectsOn ? 1 : 0); }
    private void SetViewMode(bool IsInThirdPerson){PlayerPrefs.SetInt("IsInThirdPerson", IsInThirdPerson ? 1 : 0);}
    private void SetCameraSensitivity(float sensitivity){PlayerPrefs.SetFloat("MovementCamSensitivity", sensitivity);}
    private void SetBrightness(float sensitivity){PlayerPrefs.SetFloat("lightBrightness", sensitivity);}
    private void SetMasterVolume(float volume){PlayerPrefs.SetFloat("SoundMasterVol", volume);}
    private void SetMusicVolume(float volume){PlayerPrefs.SetFloat("SoundMusicVol", volume);}
    private void SetSFXVolume(float volume){PlayerPrefs.SetFloat("SoundSFXVol", volume);}
    private void ToggleFullscreen(){Screen.fullScreen = !Screen.fullScreen;} // Tells The Browser To Go Fulscreen
    private void ToggleSaveGame() // This Code Handles The SaveGame Feature
    {
        // Make Sure We are in a level that allows saving
        if (GameManager.Instance.PreviousScene == "Level 0" || 
            GameManager.Instance.PreviousScene == "Level 1" || 
            GameManager.Instance.PreviousScene == "Level 2")
        {
            // Get The Saved Player Location Saved Before Coming To Settings
            Vector3 currentPlayerLocation = new Vector3(
                PlayerPrefs.GetFloat("templocation_x", 0),
                PlayerPrefs.GetFloat("templocation_y", 0),
                PlayerPrefs.GetFloat("templocation_z", 0)
            );

            // Tell GameManager to Update The Location Of Player
            GameManager.Instance.UpdatePlayerLocation(currentPlayerLocation);

            // Saves Players State And Calls SaveManager To Save The Game To PlayerPref
            GameManager.Instance.SavePlayerState();
            SaveManager.SaveAllData();
            
        }

    }


    private void getfromPlayerPref() // Used When Coming Back To Settings, Restores Settings From PlayerPref
    {
        // Gets Settings From PlayerPrefs And Reinstates All the settings
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

        float tempbrightness = PlayerPrefs.GetFloat("lightBrightness", 0f);
        brightness.value = tempbrightness;
        SetBrightness(tempbrightness);

        float tempmastervol = PlayerPrefs.GetFloat("SoundMasterVol", 0.5f);
        MasterVol.value = tempmastervol;
        SetMasterVolume(tempmastervol);

        float tempmusicvol = PlayerPrefs.GetFloat("SoundMusicVol", 0.5f);
        MusicVol.value = tempmusicvol;
        SetMusicVolume(tempmusicvol);

        float tempsfxvol = PlayerPrefs.GetFloat("SoundSFXVol", 0.5f);
        sfxVol.value = tempsfxvol;
        SetSFXVolume(tempsfxvol);

        // Updates The Button Groups To Activate The Correct Button Based On PlayerPrefs Recorded Data
        UpdateButtonInGroup(BattleDifficultyModes, tempdifficulty);
        UpdateButtonInGroup(ShadowToggler, tempshadow ? 0 : 1);
        UpdateButtonInGroup(ParticleToggler, tempparticle ? 0 : 1);
        UpdateButtonInGroup(EffectToggler, tempeffect ? 0 : 1);
        UpdateButtonInGroup(CameraPOVModes, temppov ? 0 : 1);
    }
}
