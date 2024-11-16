using UnityEngine;

public class CompanionAI : MonoBehaviour
{
    // set up variables to access and assign later
    public Transform CurrentPlayer; 
    public GameObject floatingText; 
    private bool isBefriended = false;
    private Rigidbody CompanionRidigbody;

    // start up the rigidbody
    void Start()
    {
        CompanionRidigbody = GetComponent<Rigidbody>();
        CompanionRidigbody.freezeRotation = true;
    }
    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false); 
            // figure out where the Companion needs to be
            Vector3 CompanionLocation = CurrentPlayer.position - CurrentPlayer.forward * 3.0f;
            CompanionLocation.y = transform.position.y; 
            CompanionLocation.y = 0;

            // Move towards the target based on their location and the distance offset we set
            Vector3 direction = (CompanionLocation - transform.position).normalized;
            CompanionRidigbody.MovePosition(transform.position + direction * 10f * Time.deltaTime);
        }
    }

    // removes text and befriends the companion to the player
    public void Befriend()
    {
        floatingText.SetActive(false); 
        isBefriended = true;
    }
}
