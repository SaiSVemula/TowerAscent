using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueNode
{
    public string NPCDialogue;
    public List<PlayerResponse> PlayerResponses;
    public DialogueNode NextNode;
}

[System.Serializable]
public class PlayerResponse
{
    public string ResponseText;
    public DialogueNode NextNode;
}

public class GeneralNPC : MonoBehaviour
{
    public Animator animator;

    [Header("Objects to Disable and Conditions")]
    public GameObject[] objectsToDisable;
    public string[] disableConditions;

    [Header("Barrier Interaction")]
    public TextMeshProUGUI barrierMessageText;
    public float messageDuration = 2f;

    [Header("Floating Text and Dialogue")]
    public string npcName = "NPC1";
    public GameObject floatingText;
    public GameObject dialogueUI;
    public Text dialogueText;
    public Transform responsePanel;
    public GameObject responseButtonPrefab;

    [Header("Detection Settings")]
    public float detectionRadius = 10f;
    private Transform player;

    [Header("Dialogue Settings")]
    public List<DialogueNode> dialogueNodes;
    private DialogueNode currentDialogueNode;
    private bool isInteracting = false;

    public float subtitleDisplayDuration = 3f;

    private ObjectiveManager objectiveManager;
    private bool isMessageDisplayed = false;

    private void Start()
    {
        floatingText?.SetActive(false);
        dialogueUI?.SetActive(false);

        ClearResponsePanel();

        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");

        objectiveManager = FindObjectOfType<ObjectiveManager>();
        if (objectiveManager == null)
        {
            Debug.LogError("ObjectiveManager not found in the scene!");
        }

        if (barrierMessageText != null)
        {
            barrierMessageText.text = ""; // Clear the barrier message at the start
        }

        CheckAndDisableObjects();
    }

    private void Update()
    {
        if (player == null || isInteracting) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            floatingText?.SetActive(true);

            FacePlayer();

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartInteraction();
            }
        }
        else
        {
            floatingText?.SetActive(false);
            if (isInteracting) EndInteraction();
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void StartInteraction()
    {
        isInteracting = true;
        floatingText?.SetActive(false);
        dialogueUI?.SetActive(true);

        animator.SetTrigger("WaveTrigger");

        if (dialogueNodes.Count > 0)
        {
            currentDialogueNode = dialogueNodes[0];
            DisplayDialogueNode(currentDialogueNode);

            HandleObjectiveAndConditions();
        }
        else
        {
            Debug.LogWarning("No dialogue nodes assigned to this NPC.");
            EndInteraction();
        }
    }

    private void HandleObjectiveAndConditions()
    {
        if (objectiveManager != null)
        {
            objectiveManager.CompleteCurrentObjective();
        }

        // Check all conditions, including spider defeat
        CheckAndDisableObjects();
    }

    private void CheckAndDisableObjects()
    {
        for (int i = 0; i < objectsToDisable.Length; i++)
        {
            if (i < disableConditions.Length && objectsToDisable[i] != null)
            {
                string condition = disableConditions[i];

                if (condition == "TalkToNPC2" && objectiveManager.IsObjectiveComplete("Talk to NPC2"))
                {
                    objectsToDisable[i].SetActive(false);
                    Debug.Log($"Disabled {objectsToDisable[i].name} for condition {condition}");
                }
                else if (condition == "DefeatSpider" && GameManager.Instance.IsSpiderDefeated("Spider1"))
                {
                    objectsToDisable[i].SetActive(false);
                    Debug.Log($"Disabled {objectsToDisable[i].name} for condition {condition}");
                }
            }
        }
    }


    public void HandleBarrierCollision()
    {
        if (!isMessageDisplayed && barrierMessageText != null)
        {
            StartCoroutine(DisplayBarrierMessage());
        }
    }

    private IEnumerator DisplayBarrierMessage()
    {
        isMessageDisplayed = true;
        barrierMessageText.text = "Required objective not reached!";
        yield return new WaitForSeconds(messageDuration);
        barrierMessageText.text = ""; // Clear the message
        isMessageDisplayed = false;
    }

    public void EndInteraction()
    {
        isInteracting = false;
        dialogueUI?.SetActive(false);
        ClearResponsePanel();
    }

    private void DisplayDialogueNode(DialogueNode node)
    {
        if (node == null) return;

        dialogueText.text = $"{npcName}: {node.NPCDialogue}";
        Invoke(node.PlayerResponses.Count > 0 ? nameof(DisplayPlayerResponses) : nameof(ProceedToNextDialogue), subtitleDisplayDuration);
    }

    private void ProceedToNextDialogue()
    {
        if (currentDialogueNode?.NextNode != null)
        {
            currentDialogueNode = currentDialogueNode.NextNode;
            DisplayDialogueNode(currentDialogueNode);
        }
        else
        {
            EndInteraction();
        }
    }

    private void DisplayPlayerResponses()
    {
        ClearResponsePanel();

        foreach (PlayerResponse response in currentDialogueNode.PlayerResponses)
        {
            GameObject button = Instantiate(responseButtonPrefab, responsePanel);
            button.SetActive(true);

            var tmpText = button.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) tmpText.text = response.ResponseText;

            button.GetComponent<Button>().onClick.AddListener(() => HandlePlayerResponse(response));
        }

        responsePanel.gameObject.SetActive(true);
    }

    private void ClearResponsePanel()
    {
        foreach (Transform child in responsePanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void HandlePlayerResponse(PlayerResponse response)
    {
        ClearResponsePanel();
        dialogueText.text = $"You: {response.ResponseText}";
        currentDialogueNode = response.NextNode;
        Invoke(nameof(DisplayNextNPCDialogue), subtitleDisplayDuration);
    }

    private void DisplayNextNPCDialogue()
    {
        DisplayDialogueNode(currentDialogueNode);
    }
}
