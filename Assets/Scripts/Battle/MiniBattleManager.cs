using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class MiniBattleManager : MonoBehaviour
{
    public static MiniBattleManager Instance;

    public GameObject miniBattleUI;
    public TMPro.TextMeshProUGUI turnIndicator;

    private GeneralMonsterAI currentSpider;
    [SerializeField] public GameObject PlayerObject;
    [SerializeField] public PlayerBattle playerBattle;
    private bool isPlayerTurn = true;

    public Transform cardPanel;
    public GameObject cardPrefab;

    // GameObjects to disable
    [SerializeField] public Button SettingsButton;
    [SerializeField] public Button BagButton;
    [SerializeField] public Canvas ObjectivesUI;

    [SerializeField] public CameraMovement cameraMovement;
    [SerializeField] private Transform originalCamera;
    [SerializeField] private Transform battleCamera; // Camera to adjust look during battle


    private void start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartMiniBattle(GeneralMonsterAI spider)
    {
        Debug.Log("Starting mini-battle with spider: " + spider.name);
        currentSpider = spider;
        Transform spiderTransform = spider.transform;

        // Disable UI elements
        SettingsButton.gameObject.SetActive(false);
        BagButton.gameObject.SetActive(false);
        ObjectivesUI.gameObject.SetActive(false);

        // Enable Mini-Battle UI
        miniBattleUI.SetActive(true);

        //PlayerObject.AddComponent<PlayerBattle>();
        playerBattle = FindObjectOfType<PlayerBattle>();
        playerBattle.SetUpMiniBattle();
        currentSpider.SetUpMiniBattle();

        PositionCameraBetweenSpiderAndPlayer(spider.transform, playerBattle.transform);

        playerBattle.LoadMiniBattleCardPool();

        // Disable camera movement
        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        Debug.Log($"player health at start : {playerBattle.CurrentHealth}");
        Debug.Log($"spider health at start : {currentSpider.CurrentHealth}");

        // Render Cards in CardsPanel
        RenderCard();

        isPlayerTurn = true;
        UpdateTurnIndicator();
        RotateCameraTowardsSpider(spiderTransform);
    }

    public void EndMiniBattle(bool playerWon)
    {
        miniBattleUI.SetActive(false);

        if (playerWon)
        {
            Debug.Log("Player won the mini-battle!");
            GameManager.Instance.AddMiniBattleWin(currentSpider.spiderID, 50);
            currentSpider.DefeatSpider();
        }
        else
        {
            Debug.Log("Player lost the mini-battle...");
            GameManager.Instance.AddMiniBattleLoss();
        }

        // Re-enable player's movement
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        ResetCameraAfterBattle();

        // Re-enable UI elements
        SettingsButton.gameObject.SetActive(true);
        BagButton.gameObject.SetActive(true);
        ObjectivesUI.gameObject.SetActive(true);
    }

    private void RotateCameraTowardsSpider(Transform spiderTransform)
    {
        if (battleCamera == null || spiderTransform == null)
        {
            return;
        }

        // Disable player camera control
        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        // Position and orient the camera towards the spider
        Vector3 offset = new Vector3(0, 5, -10); // Adjust this offset as needed
        battleCamera.position = spiderTransform.position + offset;

        // Smoothly rotate the camera to look at the spider
        Quaternion targetRotation = Quaternion.LookRotation(spiderTransform.position - battleCamera.position);
        battleCamera.rotation = Quaternion.Slerp(battleCamera.rotation, targetRotation, Time.deltaTime * 2f);

        Debug.Log("Camera temporarily focused on the spider.");
    }


    private void ResetCameraAfterBattle()
    {
        if (cameraMovement != null)
        {
            cameraMovement.enabled = true; // Re-enable player camera control
        }

        Debug.Log("Camera control reset to the player.");
    }


    private void PositionCameraBetweenSpiderAndPlayer(Transform spiderTransform, Transform playerTransform)
    {
        if (battleCamera == null || spiderTransform == null || playerTransform == null)
        {
            return;
        }

        // Calculate the midpoint between the spider and the player
        Vector3 midpoint = (spiderTransform.position + playerTransform.position) / 2;

        // Adjust camera height
        float heightAboveGround = 5f; // Adjust for camera elevation
        midpoint.y += heightAboveGround;

        // Position the camera at the midpoint
        battleCamera.position = midpoint;

        // Make the camera look at the spider
        battleCamera.LookAt(spiderTransform.position);

        Debug.Log("Camera positioned between the spider and the player.");
    }


    private void RenderCard()
    {
        Debug.Log("Rendering card in the mini-battle...");

        // Ensure the parent panel is active
        if (!cardPanel.gameObject.activeSelf)
        {
            cardPanel.gameObject.SetActive(true);
        }

        // Clear the existing card (if any)
        foreach (Transform child in cardPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Get the single card from playerBattle
        Card card = playerBattle.GetMiniBattleCard();
        if (card == null)
        {
            Debug.LogWarning("No card available in the mini-battle.");
            return;
        }

        // Instantiate the card prefab and initialize it
        GameObject cardObject = Instantiate(cardPrefab, cardPanel.transform);

        // Ensure the card object is active
        cardObject.SetActive(true);

        // Initialize the card display
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        if (cardDisplay != null)
        {
            cardDisplay.enabled = true; // Enable the CardDisplay script
            cardDisplay.Initialize(card); // Initialize the card display
        }
        else
        {
            Debug.LogError("CardDisplay component missing on cardPrefab.");
        }

        // Enable the Button component and ensure it's interactable
        Button cardButton = cardObject.GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.enabled = true; // Enable the Button script
            cardButton.interactable = true; // Ensure the button is interactable
            cardButton.onClick.AddListener(() => OnCardSelected(card));
        }
        else
        {
            Debug.LogError("Button component missing on cardPrefab.");
        }

        // Enable the Image component to render the card visually
        Image cardImage = cardObject.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.enabled = true; // Enable the Image component
        }
        else
        {
            Debug.LogError("Image component missing on cardPrefab.");
        }

        Debug.Log("Card rendered successfully.");
    }


    private void OnCardSelected(Card selectedCard)
    {
        if (!isPlayerTurn) return;

        // Use the card and apply its effect on the enemy
        string result = selectedCard.Use(playerBattle, currentSpider);
        Debug.Log(result);

        //enemyHealthBar.value = currentSpider.CurrentHealth;

        if (currentSpider.CurrentHealth <= 0)
        {
            EndMiniBattle(true);
            return;
        }

        isPlayerTurn = false;
        StartCoroutine(EnemyTurn());
    }

    public void OnWeaponCardButton()
    {
        if (!isPlayerTurn) return;

        // Fetch the player's selected card
        Card selectedCard = playerBattle.GetMiniBattleCard();
        if (selectedCard != null)
        {
            // Apply the card's effect
            string actionMessage = selectedCard.Use(playerBattle, currentSpider);
            Debug.Log(actionMessage);

            // Check if the enemy is defeated
            if (currentSpider.CurrentHealth <= 0)
            {
                EndMiniBattle(true);
                return;
            }

            // Switch to the enemy's turn
            isPlayerTurn = false;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            Debug.LogWarning("No valid card available in the player's card pool!");
        }
    }


    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        currentSpider.AttackPlayer(playerBattle);

        if (playerBattle.CurrentHealth <= 0)
        {
            EndMiniBattle(false);
            yield break;
        }

        isPlayerTurn = true;
        UpdateTurnIndicator();
    }

    private void UpdateTurnIndicator()
    {
        turnIndicator.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
    }
}
