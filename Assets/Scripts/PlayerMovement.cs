using UnityEngine;
using UnityEngine.InputSystem;
/*
This code will handle the players movement, based on Lab 2 script, 
improves the handling of physics by allowing jump and run while also fixing some
bugs such as faster diagnoal travel and player bopping up and down while moving
*/
public class PlayerMovement : MonoBehaviour
{
    //The start of this code simply declares varibles and initilaises thr movement variables with our predefined walk and run speeds

    private bool PlayerOnTheFloor;
    private PlayerControls playerControls;
    public Transform PlayerMainCam;
    private Rigidbody PlayerRigidBody;
    public Animator animator;
    public Transform CurrentPlayer;

    void Start() // Initialize variables in Start method
    {
        PlayerRigidBody = GetComponent<Rigidbody>();
        playerControls = new PlayerControls();
        playerControls.Player.Enable(); // Enable input actions
        PlayerRigidBody.freezeRotation = true;
    }

    void Update() // will read the players inputs and will fill up variables as needed and will call move and jump methods as needed
    {
        Vector2 UsersKeyStroke = playerControls.Player.MakePlayerMove.ReadValue<Vector2>();
        bool CurrentlyRunning = playerControls.Player.MakePlayerRun.IsPressed(); // is the left shift pressed to indicate running?
        bool JumpPressed = playerControls.Player.MakePlayerJump.triggered; // is space pressed to indicate user wants to jump?
        MakePlayerMove(UsersKeyStroke, CurrentlyRunning);
        if (JumpPressed) MakePlayerJump();
        UpdateAnimator(UsersKeyStroke, CurrentlyRunning); 
    }

    private void MakePlayerMove(Vector2 UsersKeyStroke, bool CurrentlyRunning) // code to move player and apply walk or run speed
    {
        Vector3 MovementLocation = (PlayerMainCam.forward * UsersKeyStroke.y + PlayerMainCam.right * UsersKeyStroke.x).normalized;
        MovementLocation.y = 0; // stops the player from bopping up and down as they move

        float SpeedOfMovement = CurrentlyRunning ? 10f : 5f;

        PlayerRigidBody.MovePosition(transform.position + MovementLocation * SpeedOfMovement * Time.deltaTime);

        Quaternion targetRotation = CurrentPlayer.localRotation;

        // Helps player rotate when WA and WD is pressed
        if (UsersKeyStroke.magnitude > 0)
        {
            Quaternion movementRotation = Quaternion.LookRotation(MovementLocation);
            targetRotation = Quaternion.Slerp(targetRotation, movementRotation, 5f * Time.deltaTime);
        }


        // Update player's rotation
        CurrentPlayer.localRotation = targetRotation;
    }

    //Makes a player jump by first checking whether the player is not already jumping (by checking if player is not touching the floor)
    private void MakePlayerJump() {
        if (PlayerOnTheFloor) { 
            PlayerRigidBody.AddForce(Vector3.up * 4.5f, ForceMode.Impulse); 

        }
        // Update jump animation
        animator.SetTrigger("Jump");
    }

    // checks if player is touching / not touching floor
    void OnCollisionEnter(Collision Touching) { if (Touching.gameObject.CompareTag("Floor")) { PlayerOnTheFloor = true; } }
    void OnCollisionExit(Collision Touching) { if (Touching.gameObject.CompareTag("Floor")) { PlayerOnTheFloor = false; } }

    private void UpdateAnimator(Vector2 UsersKeyStroke, bool CurrentlyRunning)
    {
        // Idle
        animator.SetBool("IsIdle", UsersKeyStroke.magnitude == 0);

        // Update jog animation
        animator.SetBool("WASDPressed", UsersKeyStroke.magnitude != 0);

        // Set direction - run forward and left/right
        animator.SetFloat("Direction", UsersKeyStroke.x);

        // Update run animation
        animator.SetBool("IsRunning", CurrentlyRunning);
    }
}
