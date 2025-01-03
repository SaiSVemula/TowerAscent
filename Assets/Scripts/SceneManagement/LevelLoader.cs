using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] public Animator transition;
    [SerializeField] public float transitionTime = 1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check if the scene needs to be changed
    }

    public void LoadSettingsPanel()
    {
        GameManager.Instance.PreviousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SettingsPage");
    }

    //called after the battle is over to transition to the next scene depending on the result of the battle.
    public void AfterBattleTransition(string result)
    {
        if(result == "Victory")
        {
            StartCoroutine(LoadLevel(GameManager.Instance.NextScene));
        }
        else
        {
            StartCoroutine(LoadLevel(GameManager.Instance.PreviousScene));
        }
    }

    //Helper method to load the next scene saving the previous scene in the GameManager
    public void LoadScene(string currentScene, string nextScene)
    {
        if (currentScene == "Level 0" || currentScene == "StartPage" || currentScene == "Level 1" || currentScene == "Level 2") 
        {
            GameManager.Instance.PreviousScene = currentScene;
        }
        StartCoroutine(LoadLevel(nextScene));
    }

    //Transition method that fades in and out of the scene
    IEnumerator LoadLevel(string nextScene)
    {
        Debug.Log("Loading scene: " + nextScene);
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
    }
}
