using UnityEngine;
using UnityEngine.UI;

public class CompanionAI : MonoBehaviour
{
    public Transform CurrentPlayer;
    public GameObject floatingText;
    public GameObject companionDialogueTextObject;
    public Text companionDialogueText;
    public string[] randomDialogueLines;

    private bool isBefriended = false;
    private Rigidbody CompanionRigidbody;
    private Animator animator;

    public float DefaultFollowDistance = 3f;
    public float FollowSpeed = 5f;
    public float CatchUpSpeed = 8f;
    public float FarDistanceThreshold = 6f;
    public float StoppingDistance = 1.8f;
    public float RotationSpeed = 5f;

    private Coroutine dialogueCoroutine;
    [SerializeField] private CompanionCard companionCard;

    void Start()
    {
        CompanionRigidbody = GetComponent<Rigidbody>();
        CompanionRigidbody.freezeRotation = true;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from the companion.");
        }

        companionDialogueTextObject.SetActive(false);

        if (GameManager.Instance.GetOwnedCompanions().Contains(companionCard))
        {
            isBefriended = true;
            floatingText.SetActive(false);
            Debug.Log($"{companionCard.CompanionName} is already befriended.");
        }
    }

    void Update()
    {
        if (isBefriended)
        {
            floatingText.SetActive(false);
            FollowPlayer();
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    private void FollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, CurrentPlayer.position);

        if (distanceToPlayer <= StoppingDistance)
        {
            CompanionRigidbody.velocity = Vector3.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float speed = (distanceToPlayer > FarDistanceThreshold) ? CatchUpSpeed : FollowSpeed;

        Vector3 direction = (CurrentPlayer.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotationSpeed);

        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;
        CompanionRigidbody.MovePosition(newPosition);

        float normalizedSpeed = Mathf.Clamp(speed / CatchUpSpeed, 0f, 1f);
        animator.SetFloat("Speed", normalizedSpeed);
    }

    public void Befriend()
    {
        if (isBefriended)
        {
            Debug.LogWarning($"{companionCard.CompanionName} is already befriended and cannot be added again.");
            return;
        }

        if (companionCard == null)
        {
            Debug.LogError("CompanionCard is null! Cannot befriend.");
            return;
        }

        // Add the companion to GameManager's list
        if (GameManager.Instance.GetOwnedCompanions().Contains(companionCard))
        {
            Debug.LogWarning($"{companionCard.CompanionName} is already in the owned list.");
            isBefriended = true; // Mark as befriended for follow logic
            floatingText.SetActive(false);
            return;
        }

        floatingText.SetActive(false);
        isBefriended = true;
        GameManager.Instance.AddCompanion(companionCard);

        Debug.Log($"Companion befriended: {companionCard.CompanionName}. Total companions: {GameManager.Instance.GetOwnedCompanions().Count}");
    }
}
