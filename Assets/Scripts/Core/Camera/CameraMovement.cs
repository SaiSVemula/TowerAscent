using UnityEngine;
using UnityEngine.InputSystem;

/*This code is used to allow the player to move the camera
 view around, using unity's new input system via mouse delta*/
public class CameraMovement : MonoBehaviour
{
    public Transform CurrentPlayer;
    public float rotationSpeed = 0.1f; // Speed of camera rotation

    private PlayerControls playerControls; // our input compiled script
    private float xAxisRotation = 0f; 
    private float yAxisRotation = 0f;

    void Start() // enables the input script when starting the code
    {
        playerControls = new PlayerControls();
        playerControls.Player.Enable(); // Enable input actions
    }

    void Update()
    {
        // Always follow the player
        Vector3 targetPosition = CurrentPlayer.position - Quaternion.Euler(xAxisRotation, yAxisRotation, 0) * Vector3.forward * 12;
        targetPosition.y += 5; // Add height offset
        transform.position = targetPosition;

        // Check if the left mouse button is held down
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mouseInput = playerControls.Player.Look.ReadValue<Vector2>();
            yAxisRotation += mouseInput.x * rotationSpeed;
            xAxisRotation -= mouseInput.y * rotationSpeed;
            xAxisRotation = Mathf.Clamp(xAxisRotation, -20f, 30f);
        }

        // Apply rotation and look at the player
        transform.rotation = Quaternion.Euler(xAxisRotation, yAxisRotation, 0);
        transform.LookAt(CurrentPlayer);
    }
}
