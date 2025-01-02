using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DollyCartController : MonoBehaviour
{
    private LevelLoader levelLoader;
    public CinemachineDollyCart dollyCart; // Assign the dolly cart here in the Inspector.
    public float speed = 5f;

    public Light directionalLight; // Assign the Directional Light here in the Inspector (this one follows the camera).
    public Transform lookAtTarget; // Optional: A target for the light to focus on.

    public Transform cameraTransform; // Assign your camera's transform here.
    public Vector3 cameraOffset = new Vector3(0, 2, -5); // Offset: Behind and slightly above the dolly cart.

    // New fields for scene loading
    public float trackEndThreshold = 0.95f; // Threshold to consider the track "finished" (between 0 and 1)
    private float trackLength;

    // New light for scene transition (this will only be enabled before loading the scene)
    public Light transitionDirectionalLight; // Assign this new light in the Inspector for the scene transition.

    void Start()
    {
        // Find the LevelLoader instance in the current scene
        levelLoader = FindObjectOfType<LevelLoader>();

        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
        // Ensure dolly cart is assigned and calculate the track length
        if (dollyCart != null && dollyCart.m_Path != null)
        {
            trackLength = dollyCart.m_Path.PathLength;  // Get the length of the dolly cart's path
        }
        else
        {
            Debug.LogError("DollyCart or its path is not assigned.");
        }
    }

    void Update()
    {
        if (dollyCart != null)
        {
            // Move the dolly cart along the track
            dollyCart.m_Position += speed * Time.deltaTime;

            // Update the camera's position
            if (cameraTransform != null)
            {
                // Set the camera's position relative to the dolly cart's position and rotation.
                cameraTransform.position = dollyCart.transform.position + dollyCart.transform.TransformDirection(cameraOffset);

                // Optionally make the camera look at the dolly cart's forward direction or a specific target.
                if (lookAtTarget != null)
                {
                    cameraTransform.LookAt(lookAtTarget);
                }
                else
                {
                    cameraTransform.rotation = Quaternion.LookRotation(dollyCart.transform.forward);
                }
            }

            // If the Directional Light (camera-following) is assigned, update its position or rotation.
            if (directionalLight != null)
            {
                // Make the light follow the dolly cart's position.
                directionalLight.transform.position = dollyCart.transform.position;

                // Optional: Make the light look at a target or align with the dolly cart's forward direction.
                if (lookAtTarget != null)
                {
                    directionalLight.transform.LookAt(lookAtTarget);
                }
                else
                {
                    directionalLight.transform.rotation = Quaternion.LookRotation(dollyCart.transform.forward);
                }
            }

            // Check if the dolly cart has reached the end of the track
            if (dollyCart.m_Position >= trackLength * trackEndThreshold)
            {
                // Enable the separate directional light for the scene transition
                if (transitionDirectionalLight != null)
                {
                    transitionDirectionalLight.enabled = true; // Turn on the transition directional light
                }

                // Load the "ExplorationScene" scene
                LoadNextScene();
            }
        }
    }

    void LoadNextScene()
    {
        string previousScene = GameManager.Instance.PreviousScene;
        string nextScene;

        switch (previousScene)
        {
            case "ExplorationScene":
                nextScene = "Level 1";
                break;
            case "Level 1":
                nextScene = "Level 2";
                break;
            case "Level 2":
                nextScene = "EndPage";
                break;
            default:
                Debug.LogWarning($"Current scene {previousScene} does not have a defined next scene.");
                return; // Exit if there's no defined next scene
        }

        Debug.Log($"Transitioning from LevelTransitionCutScene to {nextScene}.");

        if (levelLoader != null)
        {
            levelLoader.LoadScene("LevelTransitionCutScene", nextScene);
        }
        else
        {
            Debug.LogError("LevelLoader is not assigned.");
        }
    }
}
