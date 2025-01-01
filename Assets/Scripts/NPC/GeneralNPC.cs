using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class DialogueNode
{
    public string NPCDialogue;
    public List<PlayerResponse> PlayerResponses; // Player response options
    public DialogueNode NextNode; // Allow serialization for branching
}

[System.Serializable]
public class PlayerResponse
{
    public string ResponseText; // The text of the player response
    public DialogueNode NextNode; // Allow serialization for branching
}

public class GeneralNPC : MonoBehaviour
{
    public Animator animator;

    [Header("Floating Text and Dialogue")]
    public string npcName = "NPC1"; // Name of the NPC
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
    private bool awaitingPlayerResponse = false;

    public float subtitleDisplayDuration = 3f;

    protected virtual void Start()
    {
        if (floatingText != null) floatingText.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);

        // Clear any leftover buttons or objects in the ResponsePanel
        if (responsePanel != null)
        {
            foreach (Transform child in responsePanel)
            {
                Destroy(child.gameObject);
            }
        }

        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null) Debug.LogError("Player not found. Ensure the Player object has the 'Player' tag.");
    }

    void Update()
    {
        if (player == null || isInteracting) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            OnPlayerInRange();
        }
        else
        {
            OnPlayerOutOfRange();
        }
    }

    protected virtual void OnPlayerInRange()
    {
        if (floatingText != null && !floatingText.activeSelf) floatingText.SetActive(true);

        FacePlayer();

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartInteraction();
        }
    }

    protected virtual void OnPlayerOutOfRange()
    {
        if (floatingText != null && floatingText.activeSelf) floatingText.SetActive(false);
        if (isInteracting) EndInteraction();
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

    public virtual void StartInteraction()
    {
        isInteracting = true;
        if (floatingText != null) floatingText.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(true);

        animator.SetTrigger("WaveTrigger");

        if (dialogueNodes.Count > 0)
        {
            currentDialogueNode = dialogueNodes[0];
            DisplayDialogueNode(currentDialogueNode);
        }
        else
        {
            Debug.LogWarning("No dialogue nodes assigned to this NPC.");
            EndInteraction();
        }
    }

    public virtual void EndInteraction()
    {
        isInteracting = false;
        awaitingPlayerResponse = false;
        if (dialogueUI != null) dialogueUI.SetActive(false);

        ClearResponsePanel();
    }

    private void DisplayDialogueNode(DialogueNode node)
    {
        if (node == null) return;

        if (dialogueText != null) dialogueText.text = $"{npcName}: {node.NPCDialogue}";

        awaitingPlayerResponse = node.PlayerResponses.Count > 0;

        if (awaitingPlayerResponse)
        {
            Invoke(nameof(DisplayPlayerResponses), subtitleDisplayDuration); // Delay before showing buttons
        }
        else
        {
            Invoke(nameof(ProceedToNextDialogue), subtitleDisplayDuration);
        }
    }

    private void ProceedToNextDialogue()
    {
        if (currentDialogueNode != null && currentDialogueNode.NextNode != null)
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
        if (dialogueText != null) dialogueText.text = ""; // Hide NPC dialogue

        ClearResponsePanel();

        foreach (PlayerResponse response in currentDialogueNode.PlayerResponses)
        {
            if (responseButtonPrefab == null)
            {
                Debug.LogError("ResponseButtonPrefab is not assigned.");
                continue;
            }

            GameObject button = Instantiate(responseButtonPrefab, responsePanel);
            button.SetActive(true);

            var tmpText = button.GetComponentInChildren<TMP_Text>();
            var uiText = button.GetComponentInChildren<Text>();

            if (tmpText != null)
            {
                tmpText.text = response.ResponseText;
            }
            else if (uiText != null)
            {
                uiText.text = response.ResponseText;
            }
            else
            {
                Debug.LogError("ResponseButton is missing a Text or TMP_Text component!");
            }

            Button buttonComponent = button.GetComponent<Button>();
            buttonComponent.onClick.AddListener(() => HandlePlayerResponse(response));
        }

        responsePanel.gameObject.SetActive(true);
    }

    private void ClearResponsePanel()
    {
        // Clear the response panel without triggering excessive layout updates
        foreach (Transform child in responsePanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void HandlePlayerResponse(PlayerResponse response)
    {
        if (response != null)
        {
            responsePanel.gameObject.SetActive(false);
            ClearResponsePanel();

            if (dialogueText != null) dialogueText.text = $"You: {response.ResponseText}";

            Invoke(nameof(DisplayNextNPCDialogue), subtitleDisplayDuration);

            currentDialogueNode = response.NextNode;
        }
        else
        {
            EndInteraction();
        }
    }

    private void DisplayNextNPCDialogue()
    {
        DisplayDialogueNode(currentDialogueNode);
    }
}
