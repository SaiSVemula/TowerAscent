using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] public Animator transition;
    [SerializeField] public float transitionTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //check if the scene needs to be changed
    }

    public void LoadScene(string currentScene, string nextScene)
    {
        StartCoroutine(LoadLevel(nextScene));
    }

    IEnumerator LoadLevel(string nextScene)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
    }

}
