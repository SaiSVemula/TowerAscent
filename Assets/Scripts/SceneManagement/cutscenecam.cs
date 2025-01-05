using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
This code is used to manage the cutscenes at the start of the game
and also between battles to the next level
it manages the dolly, light, camera and calls transiiton to the next scene
*/
public class DollyCartController : MonoBehaviour
{

    private LevelLoader levelLoader;
    public CinemachineDollyCart dollyCart;
    public float camSpeed = 5f; // speed of the camera
    public Light movinglight; // the light that moves with the user
    public Transform tolookAt; // object to focus on
    public Transform movingCamera; // the camera that moves with the track
    public Vector3 CameraTune = new Vector3(0, 2, -5); // fintune the cameras offset
    public float WhenToStartTransition = 0.95f; // set when the track finishes (helps to smooth transition)
    private float scenelength;
    void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
        if (dollyCart != null && dollyCart.m_Path != null) { scenelength = dollyCart.m_Path.PathLength;} // gets dollys length
    }

    void Update()
    {
        if (dollyCart != null)
        {
            dollyCart.m_Position += camSpeed * Time.deltaTime; // moves cart around

            movingCamera.position = dollyCart.transform.position + dollyCart.transform.TransformDirection(CameraTune);


            if (tolookAt != null) { movingCamera.LookAt(tolookAt); } // sets the camera orientation to look at a object (the tower)
            else { movingCamera.rotation = Quaternion.LookRotation(dollyCart.transform.forward); }

            movinglight.transform.position = dollyCart.transform.position; // also move the lighg around
            movinglight.transform.LookAt(tolookAt); // make light point at tower

            // If we are at the end of the track go to the next scene
            if (dollyCart.m_Position >= scenelength * WhenToStartTransition) { LoadNextScene(); }
        }
    }

    public void LoadNextScene() // method to figure out what the next scene is (as for battle transition we need to go to different levels)
    {
        string previousScene = GameManager.Instance.PreviousScene; // get last scene
        string nextScene;

        switch (previousScene) // find the next scene
        {
            case "StartPage":
                nextScene = "Level 0";
                break;
            case "Level 0":
                nextScene = "Level 1";
                break;
            case "Level 1":
                nextScene = "Level 2";
                break;
            case "Level 2":
                nextScene = "EndPage";
                break;
            default:
                return;
        }

        if (levelLoader != null) { levelLoader.LoadScene("LevelTransitionCutScene", nextScene); } // transition to the next scene
    }
}
