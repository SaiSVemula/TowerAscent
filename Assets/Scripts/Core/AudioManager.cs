using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    
    [Header("Audio Clips")]
    public AudioClip StartScreen;
    public AudioClip Cutscene;
    public AudioClip Level0;
    public AudioClip Level1;
    public AudioClip Level2;
    public AudioClip BossBattle;
    public AudioClip CardScreen;

    public AudioClip[] sfx;

    public string SavedScene;

    private float mastervol;
    private float musicvol;
    private float sfxvol;

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void UpdateCurrentScene() { SavedScene = SceneManager.GetActiveScene().name; }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCurrentScene();
        PlaySceneMusic(SavedScene);
    }

    void Awake()
    {
        // Implement the singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate AudioManagers
        }
    }

    void Start()
    {
        // Set up audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        SFXSource = gameObject.AddComponent<AudioSource>();

        PlayMusic(StartScreen);

    }

    void Update()
    {
        mastervol = PlayerPrefs.GetFloat("SoundMasterVol", 0.5f);
        musicvol = PlayerPrefs.GetFloat("SoundMusicVol", 0.5f);
        sfxvol = PlayerPrefs.GetFloat("SoundSFXVol", 0.5f);
        
        UpdateVolume(mastervol, musicvol, sfxvol);
    }

    public void UpdateVolume(float masterVolume, float musicVolume, float sfxVolume)
    {
        // Set the master volume scale
        float finalMusicVolume = masterVolume * musicVolume;
        float finalSFXVolume = masterVolume * sfxVolume;

        // Apply the calculated volumes to the audio sources
        musicSource.volume = finalMusicVolume;
        SFXSource.volume = finalSFXVolume;
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true; // Loop the background music
        musicSource.Play();
    }

    public void PlaySceneMusic(string sceneName)
    {

        musicSource.Stop();
        Debug.Log("Clip stopped" + musicSource.volume);

        AudioClip newClip = null;

        switch (sceneName)
        {
            case "StartCutScene":
                newClip = Cutscene;
                break;
            case "Level 0":
                newClip = Level0;
                break;
            case "Level 1":
                newClip = Level1;
                break;
            case "Level 2":
                newClip = Level2;
                break;
            case "BattleScene":
                newClip = BossBattle;
                break;
            case "LoadoutPage":
                newClip = CardScreen;
                break;
        }
        
        PlayMusic(newClip);
        Debug.Log("Clip playing" + musicSource.volume);
    }


    public void PlaySFX(int effectIndex)
    {
        if (effectIndex >= 0 && effectIndex < sfx.Length)
        {
            SFXSource.PlayOneShot(sfx[effectIndex]);
        }
    }

}
