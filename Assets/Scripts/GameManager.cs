using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string CurrentSceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        // if (Instance == null) Instance = this;
        // else Destroy(gameObject);

        // DontDestroyOnLoad(gameObject);
    }

    void UpdateSceneInfo()
    {
        // CurrentSceneName = SceneManager.GetActiveScene().name;
    }
}
