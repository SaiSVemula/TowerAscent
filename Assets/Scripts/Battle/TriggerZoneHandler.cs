using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TriggerZoneHandler : MonoBehaviour
{
    public GameObject promptPanel;
    public GameObject EnemyPill;
    public TMP_Text promptMessage;
    public Button yesButton;
    public Button noButton;
    public Transform player;
    public Vector3 outsideBoxPosition;

    private bool isPromptActive = false;
    private bool isLoading = false;
    private bool isTriggered = false;

    private LevelLoader levelLoader;

    void Start()
    {
        // Find the LevelLoader instance in the current scene
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
        // Hide the prompt panel and add listeners
        promptPanel.SetActive(false);
        yesButton.onClick.AddListener(OnYesButton);
        noButton.onClick.AddListener(OnNoButton);
    }

    // Function to handle the trigger enter event
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameObject.CompareTag("PromptZone"))
        {
            ShowPrompt();
            EnemyPill.SetActive(true);
        }
        else if (gameObject.CompareTag("ForcedZone") && !isLoading)
        {
            isTriggered = true;
            StartCoroutine(StartLoadingAnimation());
        }
    }

    // Function to show the prompt panel
    public void ShowPrompt()
    {
        if (!isTriggered)
        {
            promptPanel.SetActive(true);
            promptMessage.SetText("Do you wish to enter battle?");
            isPromptActive = true;

            // Make sure the buttons are visible
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
    }

    // Function to handle the "Yes" button click
    public void OnYesButton()
    {
        if (isPromptActive)
        {
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            // Start the loading sequence
            StartCoroutine(StartLoadingAnimation());
        }
    }

    // Function to handle the "No" button click
    public void OnNoButton()
    {
        if (isPromptActive)
        {
            // Hide both buttons
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            promptPanel.SetActive(false);
            player.position = outsideBoxPosition;
            isPromptActive = false;
        }
    }

    // Coroutine to animate the loading process and use the LevelLoader
    IEnumerator StartLoadingAnimation()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // Set the loading state to true
        isLoading = true;
        promptMessage.SetText("Loading");
        promptMessage.alignment = TextAlignmentOptions.Center;

        promptPanel.GetComponent<Image>().color = Color.red;

        string[] loadingStates = { "Loading", "Loading.", "Loading..", "Loading..." };
        int index = 0;

        // Loop through the loading animation for 5 seconds
        for (int i = 0; i < 5; i++)
        {
            promptMessage.SetText(loadingStates[index]);
            index = (index + 1) % loadingStates.Length;
            yield return new WaitForSeconds(1f);
        }

        // Set the loading state to false
        isTriggered = false;

        //getting scene names to set which scene to transition to after battle.
        string nextScene;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Level 0")
        {
            nextScene = "Level 1";
        }
        else if (currentScene == "Level 1")
        {
            nextScene = "Level 2";
        }
        else
        {
            nextScene = "EndPage";
        }

        // Use the LevelLoader to load the scene
        if (levelLoader != null)
        {
            GameManager.Instance.NextScene = nextScene;
            Debug.Log($"Next scene set to: {nextScene}");
            levelLoader.LoadScene(currentScene, "LoadoutPage");
        }
        else
        {
            Debug.LogError("LevelLoader instance not found. Ensure LevelLoader is in the scene.");
        }
    }
}
