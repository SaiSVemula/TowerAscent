using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void NewGame()
    {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene("ExplorationScene");
    }

    public void Resume()
    {
        // Add Code Here To Call Variables From Browser
    }

    public void Settings()
    {
        GameManager.Instance.SaveScene();
        SceneManager.LoadScene("SettingsPage");
    }

    public void Quit()
    {
        SceneManager.LoadScene("EndPage");
    }

}
