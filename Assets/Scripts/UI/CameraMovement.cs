using UnityEngine;
using UnityEngine.InputSystem;

/*This code is used to allow the player to move the camera
 view around, using Unity's new input system via mouse delta*/
public class CameraMovement : MonoBehaviour
{
    public Transform CurrentPlayer;
    public float rotationSpeed = 0.1f; // Speed of camera rotation
    public bool isThirdPerson = true; // Toggle via Inspector for third or first person
    public float thirdPersonDistance = 12f; // Distance from the player in third-person
    public float thirdPersonHeightOffset = 5f; // Height offset for third-person camera
    public float firstPersonHeightOffset = 1.6f; // Height for first-person camera (rough estimate)
    public float firstPersonForwardOffset = 0.5f; // Slight forward offset for first-person view
    public GameObject RPGHeroPBR; // Reference to the RPGHeroPBR object

    private PlayerControls playerControls; // our input compiled script
    private float xAxisRotation = 0f; 
    private float yAxisRotation = 0f;

    void Start() // enables the input script when starting the code
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable(); // Enable input actions
        isThirdPerson = GameManager.Instance.GetPOV();
    }

    void Update()
    {
        if (isThirdPerson)
        {
            // Enable RPGHeroPBR in third-person
            if (RPGHeroPBR != null)
            {
                RPGHeroPBR.SetActive(true);
            }

            // Third-person camera logic
            Vector3 targetPosition = CurrentPlayer.position - Quaternion.Euler(xAxisRotation, yAxisRotation, 0) * Vector3.forward * thirdPersonDistance;
            targetPosition.y += thirdPersonHeightOffset; // Add height offset
            transform.position = targetPosition;

            // Handle mouse input for rotation
            Vector2 mouseInput = playerControls.Player.Look.ReadValue<Vector2>();
            yAxisRotation += mouseInput.x * rotationSpeed;
            xAxisRotation -= mouseInput.y * rotationSpeed;
            xAxisRotation = Mathf.Clamp(xAxisRotation, -20f, 30f);

            // Apply rotation and look at the player
            transform.rotation = Quaternion.Euler(xAxisRotation, yAxisRotation, 0);
            transform.LookAt(CurrentPlayer);
        }
        else
        {
            // Disable RPGHeroPBR in first-person
            if (RPGHeroPBR != null)
            {
                RPGHeroPBR.SetActive(false);
            }

            // First-person camera logic (camera slightly forward from the player)
            Vector3 targetPosition = CurrentPlayer.position + CurrentPlayer.forward * firstPersonForwardOffset;
            targetPosition.y += firstPersonHeightOffset; // Set the height for first-person view
            transform.position = targetPosition;

            // Handle mouse input for rotation (no offset for first-person)
            Vector2 mouseInput = playerControls.Player.Look.ReadValue<Vector2>();
            yAxisRotation += mouseInput.x * rotationSpeed;
            xAxisRotation -= mouseInput.y * rotationSpeed;
            xAxisRotation = Mathf.Clamp(xAxisRotation, -80f, 80f); // Limiting rotation range for first-person view

            // Apply rotation
            transform.rotation = Quaternion.Euler(xAxisRotation, yAxisRotation, 0);
        }
    }
}
