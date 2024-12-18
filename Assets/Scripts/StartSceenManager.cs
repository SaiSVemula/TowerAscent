using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("ExplorationScene"); // Load the main game scene
    }

    public void QuitGame()
    {
        Application.Quit(); // Quit the game
    }
}
