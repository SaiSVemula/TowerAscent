using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
        floatingText.SetActive(false);
        isBefriended = true;

        // Add this companion to the party
        GameManager.Instance.AddCompanionToParty(gameObject);

        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
        dialogueCoroutine = StartCoroutine(DisplayCompanionMessages());
    }

    private IEnumerator DisplayCompanionMessages()
    {
        companionDialogueTextObject.SetActive(true);

        companionDialogueText.text = "Companion has joined your party";
        yield return new WaitForSeconds(3f);

        companionDialogueTextObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        while (true)
        {
            companionDialogueTextObject.SetActive(true);
            companionDialogueText.text = randomDialogueLines[Random.Range(0, randomDialogueLines.Length)];
            yield return new WaitForSeconds(10f);
            companionDialogueTextObject.SetActive(false);
        }
    }
}
