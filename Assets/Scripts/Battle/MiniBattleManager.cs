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

        battleCameraPosition = currentSpider.BattleCamera;

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
            GameManager.Instance.AddMiniBattleWin(currentSpider.spiderID, 10);
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
        // foreach (Transform child in cardPanel.transform)
        // {
        //     Destroy(child.gameObject);
        // }

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
        AudioManager.instance.PlaySFX(4);
        AudioManager.instance.PlaySFX(3);

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
