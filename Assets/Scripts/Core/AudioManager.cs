using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    
    [Header("Audio Clips")]
    public AudioClip StartScreen;
    public AudioClip StartCutsceneANDLevel0;
    public AudioClip BossBattle;
    public AudioClip Level2;

    public AudioClip[] sfx;

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

        // Play background music
        PlayMusic(StartScreen);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true; // Loop the background music
        musicSource.Play();
    }

    public void PlaySFX(int effectIndex)
    {
        if (effectIndex >= 0 && effectIndex < sfx.Length)
        {
            SFXSource.PlayOneShot(sfx[effectIndex]);
        }
    }

}
