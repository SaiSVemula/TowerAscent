//using System.Collections;
//using System.Collections.Generic;
//using TMPro.Examples;
//using UnityEngine;
//using UnityEngine.UI;

//public class MiniBattleManager : MonoBehaviour
//{
//    public static MiniBattleManager Instance;

//    public GameObject miniBattleUI;
//    public TMPro.TextMeshProUGUI turnIndicator;

//    private GeneralMonsterAI currentSpider;
//    [SerializeField] public GameObject PlayerObject;
//    [SerializeField] public PlayerBattle playerBattle;
//    private bool isPlayerTurn = true;

//    public Transform cardPanel;
//    public GameObject cardPrefab;

//    // GameObjects to disable
//    [SerializeField] public Button SettingsButton;
//    [SerializeField] public Button BagButton;
//    [SerializeField] public Canvas ObjectivesUI;

//    [SerializeField] public CameraMovement cameraMovement;
//    [SerializeField] private Transform originalCamera;
//    [SerializeField] private Transform battleCamera; // Camera to adjust look during battle

//    [SerializeField] private Vector3 originalCameraPosition;
//    [SerializeField] private Quaternion originalCameraRotation;

//    private void start()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    public void StartMiniBattle(GeneralMonsterAI spider)
//    {
//        Debug.Log("Starting mini-battle with spider: " + spider.name);
//        currentSpider = spider;
//        Transform spiderTransform = spider.transform;

//        // Save the original camera position and rotation
//        originalCameraPosition = battleCamera.position;
//        originalCameraRotation = battleCamera.rotation;

//        // Disable player controls (if applicable)
//        if (cameraMovement != null)
//        {
//            cameraMovement.enabled = false;
//        }

//        // Position the camera dynamically to focus on the spider
//        PositionCameraToFocusOnSpider(spiderTransform);

//        // Rest of the battle setup logic
//        SettingsButton.gameObject.SetActive(false);
//        BagButton.gameObject.SetActive(false);
//        ObjectivesUI.gameObject.SetActive(false);
//        miniBattleUI.SetActive(true);

//        playerBattle = FindObjectOfType<PlayerBattle>();
//        playerBattle.SetUpMiniBattle();
//        currentSpider.SetUpMiniBattle();

//        Debug.Log($"player health at start : {playerBattle.CurrentHealth}");
//        Debug.Log($"spider health at start : {currentSpider.CurrentHealth}");

//        playerBattle.LoadMiniBattleCardPool();
//        RenderCard();

//        isPlayerTurn = true;
//        UpdateTurnIndicator();
//    }

//    private void PositionCameraToFocusOnSpider(Transform spiderTransform)
//    {
//        if (battleCamera == null || spiderTransform == null)
//        {
//            Debug.LogError("Battle camera or spider transform is null.");
//            return;
//        }

//        // Calculate a position offset relative to the spider
//        Vector3 offset = new Vector3(0, 5, -10); // Adjust height and distance as needed
//        Vector3 cameraPosition = spiderTransform.position + offset;

//        // Set the camera position
//        battleCamera.position = cameraPosition;

//        // Rotate the camera to look at the spider
//        battleCamera.LookAt(spiderTransform);

//        Debug.Log("Camera positioned dynamically to focus on the spider.");
//    }


//    public void EndMiniBattle(bool playerWon)
//    {
//        miniBattleUI.SetActive(false);

//        if (playerWon)
//        {
//            Debug.Log("Player won the mini-battle!");
//            GameManager.Instance.AddMiniBattleWin(currentSpider.spiderID, 50);
//            currentSpider.DefeatSpider();
//        }
//        else
//        {
//            Debug.Log("Player lost the mini-battle...");
//            GameManager.Instance.AddMiniBattleLoss();
//        }

//        // Restore the original camera position and rotation
//        ResetCameraAfterBattle();

//        // Re-enable player movement and UI elements
//        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
//        if (playerMovement != null)
//        {
//            playerMovement.enabled = true;
//        }

//        SettingsButton.gameObject.SetActive(true);
//        BagButton.gameObject.SetActive(true);
//        ObjectivesUI.gameObject.SetActive(true);
//    }

//    private void ResetCameraAfterBattle()
//    {
//        if (battleCamera == null)
//        {
//            Debug.LogError("Battle camera is null.");
//            return;
//        }

//        // Restore the camera's original position and rotation
//        battleCamera.position = originalCameraPosition;
//        battleCamera.rotation = originalCameraRotation;

//        // Re-enable player camera control
//        if (cameraMovement != null)
//        {
//            cameraMovement.enabled = true;
//        }

//        Debug.Log("Camera reset to its original position and rotation.");
//    }


//    private void RotateCameraTowardsSpider(Transform spiderTransform)
//    {
//        if (battleCamera == null || spiderTransform == null)
//        {
//            return;
//        }

//        // Disable player camera control
//        if (cameraMovement != null)
//        {
//            cameraMovement.enabled = false;
//        }

//        // Position and orient the camera towards the spider
//        Vector3 offset = new Vector3(0, 5, -10); // Adjust this offset as needed
//        battleCamera.position = spiderTransform.position + offset;

//        // Smoothly rotate the camera to look at the spider
//        Quaternion targetRotation = Quaternion.LookRotation(spiderTransform.position - battleCamera.position);
//        battleCamera.rotation = Quaternion.Slerp(battleCamera.rotation, targetRotation, Time.deltaTime * 2f);

//        Debug.Log("Camera temporarily focused on the spider.");
//    }

//    private void PositionCameraBetweenSpiderAndPlayer(Transform spiderTransform, Transform playerTransform)
//    {
//        if (battleCamera == null || spiderTransform == null || playerTransform == null)
//        {
//            return;
//        }

//        // Calculate the midpoint between the spider and the player
//        Vector3 midpoint = (spiderTransform.position + playerTransform.position) / 2;

//        // Adjust camera height
//        float heightAboveGround = 5f; // Adjust for camera elevation
//        midpoint.y += heightAboveGround;

//        // Position the camera at the midpoint
//        battleCamera.position = midpoint;

//        // Make the camera look at the spider
//        battleCamera.LookAt(spiderTransform.position);

//        Debug.Log("Camera positioned between the spider and the player.");
//    }


//    private void RenderCard()
//    {
//        Debug.Log("Rendering card in the mini-battle...");

//        // Ensure the parent panel is active
//        if (!cardPanel.gameObject.activeSelf)
//        {
//            cardPanel.gameObject.SetActive(true);
//        }

//        // Clear the existing card (if any)
//        foreach (Transform child in cardPanel.transform)
//        {
//            Destroy(child.gameObject);
//        }

//        // Get the single card from playerBattle
//        Card card = playerBattle.GetMiniBattleCard();
//        if (card == null)
//        {
//            Debug.LogWarning("No card available in the mini-battle.");
//            return;
//        }

//        Debug.Log($"Rendering Card: {card.Name}, Sprite: {card.CardSprite}");

//        // Instantiate the card prefab and initialize it
//        GameObject cardObject = Instantiate(cardPrefab, cardPanel.transform);

//        if (cardObject == null)
//        {
//            Debug.LogError("Card prefab instantiation failed!");
//            return;
//        }

//        // Initialize the card display
//        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
//        if (cardDisplay != null)
//        {
//            if (card.CardSprite == null)
//            {
//                Debug.LogWarning($"Card {card.Name} has a null sprite.");
//            }

//            cardDisplay.Initialize(card); // Initialize the card display
//        }
//        else
//        {
//            Debug.LogError("CardDisplay component missing on cardPrefab.");
//            return;
//        }

//        // Ensure the Button component is enabled
//        Button cardButton = cardObject.GetComponent<Button>();
//        if (cardButton != null)
//        {
//            cardButton.interactable = true;
//            cardButton.onClick.AddListener(() => OnCardSelected(card));
//        }
//        else
//        {
//            Debug.LogError("Button component missing on cardPrefab.");
//        }

//        // Ensure the Image component is enabled
//        Image cardImage = cardObject.GetComponent<Image>();
//        if (cardImage != null)
//        {
//            if (card.CardSprite == null)
//            {
//                Debug.LogWarning($"No sprite found for card: {card.Name}. Default sprite will be used.");
//                cardImage.sprite = null; // Or assign a default sprite
//            }
//            else
//            {
//                cardImage.sprite = card.CardSprite;
//            }
//        }
//        else
//        {
//            Debug.LogError("Image component missing on cardPrefab.");
//        }

//        cardDisplay.enabled = true;
//        cardImage.enabled = true;
//        cardButton.enabled = true;
//        //cardObject.enabled = true;
//        Debug.Log("Card rendered successfully.");
//    }



//    private void OnCardSelected(Card selectedCard)
//    {
//        if (!isPlayerTurn) return;

//        // Use the card and apply its effect on the enemy
//        string result = selectedCard.Use(playerBattle, currentSpider);
//        Debug.Log(result);

//        //enemyHealthBar.value = currentSpider.CurrentHealth;

//        if (currentSpider.CurrentHealth <= 0)
//        {
//            EndMiniBattle(true);
//            return;
//        }

//        isPlayerTurn = false;
//        StartCoroutine(EnemyTurn());
//    }

//    public void OnWeaponCardButton()
//    {
//        if (!isPlayerTurn) return;

//        // Fetch the player's selected card
//        Card selectedCard = playerBattle.GetMiniBattleCard();
//        if (selectedCard != null)
//        {
//            // Apply the card's effect
//            string actionMessage = selectedCard.Use(playerBattle, currentSpider);
//            Debug.Log(actionMessage);

//            // Check if the enemy is defeated
//            if (currentSpider.CurrentHealth <= 0)
//            {
//                EndMiniBattle(true);
//                return;
//            }

//            // Switch to the enemy's turn
//            isPlayerTurn = false;
//            StartCoroutine(EnemyTurn());
//        }
//        else
//        {
//            Debug.LogWarning("No valid card available in the player's card pool!");
//        }
//    }


//    private IEnumerator EnemyTurn()
//    {
//        yield return new WaitForSeconds(1f);

//        currentSpider.AttackPlayer(playerBattle);

//        if (playerBattle.CurrentHealth <= 0)
//        {
//            EndMiniBattle(false);
//            yield break;
//        }

//        isPlayerTurn = true;
//        UpdateTurnIndicator();
//    }

//    private void UpdateTurnIndicator()
//    {
//        turnIndicator.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
//    }
//}


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniBattleManager : MonoBehaviour
{
    public static MiniBattleManager Instance;

    public GameObject miniBattleUI;
    public TMPro.TextMeshProUGUI turnIndicator;

    [SerializeField] public CameraMovement cameraMovement;
    private GeneralMonsterAI currentSpider;
    [SerializeField] public PlayerBattle playerBattle;
    private bool isPlayerTurn = true;

    public Transform cardPanel;
    public GameObject cardPrefab;

    // UI and GameObjects to disable during the battle
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button BagButton;
    [SerializeField] private Canvas ObjectivesUI;

    // Camera-related references
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform battleCameraPosition; // Predefined battle camera position
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    [SerializeField] private float transitionDuration = 1.0f; // Smooth transition duration

    private void Awake()
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

        // Save the original camera state
        SaveOriginalCameraState();

        RotateSpiderTowardsCamera(spider.transform);

        // Transition the camera to the predefined battle position
        StartCoroutine(SmoothTransitionToPosition(battleCameraPosition.position, battleCameraPosition.rotation));

        // Disable player camera controls
        if (cameraMovement != null)
        {
            cameraMovement.enabled = false;
        }

        // Disable UI and initialize mini-battle
        DisableUI();
        miniBattleUI.SetActive(true);

        playerBattle = FindObjectOfType<PlayerBattle>();
        playerBattle.SetUpMiniBattle();
        spider.SetUpMiniBattle();

        playerBattle.LoadMiniBattleCardPool();
        RenderCard();

        isPlayerTurn = true;
        UpdateTurnIndicator();
    }

    public void EndMiniBattle(bool playerWon)
    {
        Debug.Log($"Ending mini-battle. Player won: {playerWon}");

        // Restore the camera to its original position
        StartCoroutine(SmoothTransitionToPosition(originalCameraPosition, originalCameraRotation));

        // Disable player camera controls
        if (cameraMovement != null)
        {
            cameraMovement.enabled = true;
        }

        // Re-enable player movement
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Re-enable UI
        EnableUI();
        miniBattleUI.SetActive(false);

        // Handle battle outcome
        if (playerWon)
        {
            GameManager.Instance.AddMiniBattleWin(currentSpider.spiderID, 50);
            currentSpider.DefeatSpider();
        }
        else
        {
            GameManager.Instance.AddMiniBattleLoss();
        }
    }

    private void SaveOriginalCameraState()
    {
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
            originalCameraRotation = mainCamera.transform.rotation;
        }
        else
        {
            Debug.LogError("Main camera is not assigned!");
        }
    }

    private void RotateSpiderTowardsCamera(Transform spiderTransform)
    {
        if (spiderTransform == null || battleCameraPosition == null)
        {
            Debug.LogError("Spider transform or battle camera position is null!");
            return;
        }

        // Calculate the direction vector from the spider to the camera
        Vector3 directionToCamera = (battleCameraPosition.position - spiderTransform.position).normalized;

        // Remove any vertical (Y-axis) component to keep the spider horizontal
        directionToCamera.y = 0;

        // Rotate the spider to face the camera
        Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
        spiderTransform.rotation = targetRotation;

        Debug.Log("Spider is now facing the camera.");
    }


    private IEnumerator SmoothTransitionToPosition(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not assigned!");
            yield break;
        }

        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / transitionDuration);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;

        Debug.Log("Camera transition completed.");
    }

    private void DisableUI()
    {
        if (SettingsButton != null) SettingsButton.gameObject.SetActive(false);
        if (BagButton != null) BagButton.gameObject.SetActive(false);
        if (ObjectivesUI != null) ObjectivesUI.gameObject.SetActive(false);
    }

    private void EnableUI()
    {
        if (SettingsButton != null) SettingsButton.gameObject.SetActive(true);
        if (BagButton != null) BagButton.gameObject.SetActive(true);
        if (ObjectivesUI != null) ObjectivesUI.gameObject.SetActive(true);
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

        Debug.Log($"Rendering Card: {card.Name}, Sprite: {card.CardSprite}");

        // Instantiate the card prefab and initialize it
        GameObject cardObject = Instantiate(cardPrefab, cardPanel.transform);

        if (cardObject == null)
        {
            Debug.LogError("Card prefab instantiation failed!");
            return;
        }

        // Initialize the card display
        CardDisplay cardDisplay = cardObject.GetComponent<CardDisplay>();
        if (cardDisplay != null)
        {
            if (card.CardSprite == null)
            {
                Debug.LogWarning($"Card {card.Name} has a null sprite.");
            }

            cardDisplay.Initialize(card); // Initialize the card display
        }
        else
        {
            Debug.LogError("CardDisplay component missing on cardPrefab.");
            return;
        }

        // Ensure the Button component is enabled
        Button cardButton = cardObject.GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.interactable = true;
            cardButton.onClick.AddListener(() => OnCardSelected(card));
        }
        else
        {
            Debug.LogError("Button component missing on cardPrefab.");
        }

        // Ensure the Image component is enabled
        Image cardImage = cardObject.GetComponent<Image>();
        if (cardImage != null)
        {
            if (card.CardSprite == null)
            {
                Debug.LogWarning($"No sprite found for card: {card.Name}. Default sprite will be used.");
                cardImage.sprite = null; // Or assign a default sprite
            }
            else
            {
                cardImage.sprite = card.CardSprite;
            }
        }
        else
        {
            Debug.LogError("Image component missing on cardPrefab.");
        }

        cardDisplay.enabled = true;
        cardImage.enabled = true;
        cardButton.enabled = true;
        //cardObject.enabled = true;
        Debug.Log("Card rendered successfully.");
    }

    private void OnCardSelected(Card selectedCard)
    {
        if (!isPlayerTurn) return;

        string result = selectedCard.Use(playerBattle, currentSpider);
        Debug.Log(result);

        if (currentSpider.CurrentHealth <= 0)
        {
            EndMiniBattle(true);
            return;
        }

        isPlayerTurn = false;
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f);

        currentSpider.AttackPlayer(playerBattle);

        if (playerBattle.CurrentHealth <= 0)
        {
            EndMiniBattle(false);
        }
        else
        {
            isPlayerTurn = true;
            UpdateTurnIndicator();
        }
    }

    private void UpdateTurnIndicator()
    {
        turnIndicator.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
    }
}
