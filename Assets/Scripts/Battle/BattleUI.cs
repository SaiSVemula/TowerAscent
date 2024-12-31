using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{

    [Header("UI Canvases")]
    [SerializeField] private Canvas instructionCanvas;
    [SerializeField] private Canvas battleCanvas;

    [Header("Instructions")]
    [SerializeField] private TextMeshProUGUI gameInstructionText;
    [SerializeField] private Button startBattleButton;

    [Header("Card System")]
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardTemplate;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private PlayerBattle playerBattle;
    [SerializeField] private EnemyBattle enemyBattle;

    [Header("Battle Log")]
    [SerializeField] private ScrollRect battleLogScrollRect;
    [SerializeField] private TextMeshProUGUI logEntryTemplate;
    [SerializeField] private RectTransform logContainer;
    [SerializeField] private float logEntrySpacing = 10f;

    [Header("Status Effects")]
    [SerializeField] private TextMeshProUGUI defenceTimerText;
    [SerializeField] private TextMeshProUGUI healingTimerText;

    private Queue<TextMeshProUGUI> logPool = new Queue<TextMeshProUGUI>();
    private List<TextMeshProUGUI> activeLogEntries = new List<TextMeshProUGUI>();
    private List<GameObject> renderedCards = new List<GameObject>();
    private const int MAX_LOG_ENTRIES = 50;
    private const int POOL_SIZE = 60;

    [Header("Turn Indicator")]
    [SerializeField] private TextMeshProUGUI turnIndicatorText;
    [SerializeField] private Image turnIndicatorBackground;
    [SerializeField] private Color playerTurnColor = new Color(0.2f, 0.6f, 1f, 1f); // Light blue
    [SerializeField] private Color enemyTurnColor = new Color(1f, 0.3f, 0.3f, 1f);  // Light red

    [Header("Game Status")]
    [SerializeField] private TextMeshProUGUI gameStatusText;
    private bool isInitialized = false;

    // Reference to the LevelLoader instance in the scene

    private void Awake()
    {
        Debug.Log("BattleUI Awake");
        // Find the LevelLoader instance in the current scene

        // Initially show only instruction canvas
        instructionCanvas.enabled = true;
        battleCanvas.enabled = false;


        SetupInstructions();
        isInitialized = true;
        Debug.Log("BattleUI Initialized");
    }

    public bool IsBattleReady { get; private set; } = false;

    private void OnStartBattleClick()
    {
        if (!isInitialized)
        {
            Debug.LogError("BattleUI not initialized!");
            return;
        }

        Debug.Log("Start Battle button clicked");

        // Disable instruction canvas
        instructionCanvas.enabled = false;

        // Signal that the battle is ready to begin
        IsBattleReady = true;

        // Enable game status text
        gameStatusText.enabled = true;

        // Start battle initialization
        if (battleManager != null)
        {
            StartCoroutine(battleManager.InitializeBattle());
        }
        else
        {
            Debug.LogError("BattleManager reference missing!");
        }
    }


    private void SetupInstructions()
    {
        if (startBattleButton == null || gameInstructionText == null)
        {
            Debug.LogError("Missing critical references!");
            return;
        }

        gameInstructionText.text = "Battle Instructions:\n\n" +
            "� Click cards to use them\n" +
            "� Attack cards deal damage\n" +
            "� Defense cards block damage\n" +
            "� Healing cards restore health\n" +
            "� Take turns with the enemy\n\n" +
            "Click 'Start Battle' to begin!";

        startBattleButton.onClick.RemoveAllListeners();
        startBattleButton.onClick.AddListener(OnStartBattleClick);
    }

    private void Start()
    {
        SetupScrollView();
        InitializeLogPool();
    }

    private void SetupScrollView()
    {
        VerticalLayoutGroup verticalLayout = logContainer.GetComponent<VerticalLayoutGroup>();
        if (verticalLayout == null)
        {
            verticalLayout = logContainer.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayout.spacing = logEntrySpacing;
            verticalLayout.childAlignment = TextAnchor.UpperCenter;
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = true;
        }

        ContentSizeFitter contentSizeFitter = logContainer.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter == null)
        {
            contentSizeFitter = logContainer.gameObject.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }

    private void InitializeLogPool()
    {
        logEntryTemplate.gameObject.SetActive(false);
        for (int i = 0; i < POOL_SIZE; i++)
        {
            TextMeshProUGUI newEntry = Instantiate(logEntryTemplate, logContainer);
            newEntry.gameObject.SetActive(false);
            logPool.Enqueue(newEntry);
        }
    }

    public void RenderCards(List<Card> cards)
    {
        // Clear existing cards
        foreach (GameObject cardObject in renderedCards)
        {
            Destroy(cardObject);
        }
        renderedCards.Clear();

        // Layout calculations
        float cardWidth = 150f;
        float cardHeight = 200f;
        float spacing = 20f;
        float startX = -(cards.Count * (cardWidth + spacing)) / 2 + (cardWidth / 2);

        // Create and setup cards
        for (int i = 0; i < cards.Count; i++)
        {
            Card currentCard = cards[i];
            GameObject cardObject = Instantiate(cardTemplate, cardContainer);
            cardObject.SetActive(true);

            // Setup RectTransform
            RectTransform rectTransform = cardObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(cardWidth, cardHeight);
                rectTransform.localScale = Vector3.one * 0.7f;
                rectTransform.localPosition = new Vector3(startX + i * (cardWidth + spacing), 0, 0);
            }

            // Setup card visuals
            Image cardImage = cardObject.GetComponent<Image>();
            if (cardImage != null && currentCard.CardSprite != null)
            {
                cardImage.sprite = currentCard.CardSprite;
            }

            // Setup card name
            Text cardNameText = cardObject.GetComponentInChildren<Text>();
            if (cardNameText != null)
            {
                cardNameText.text = currentCard.Name;
            }

            // Setup button and handle card states
            Button cardButton = cardObject.GetComponent<Button>();
            if (cardButton != null)
            {
                int index = i;
                bool isDisabled = false;

                // Check if card should be disabled
                if (currentCard is DefenceCard && playerBattle.TemporaryDefences.Count > 0)
                {
                    isDisabled = true;
                }
                else if (currentCard is HealingCard && playerBattle.TemporaryHeals.Count > 0)
                {
                    isDisabled = true;
                }

                // Set button state
                cardButton.interactable = !isDisabled;

                // Visual feedback for disabled state
                if (isDisabled)
                {
                    cardImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                else
                {
                    cardImage.color = Color.white;
                }

                cardButton.onClick.AddListener(() => battleManager.OnPlayerUseCard(index));
            }

            renderedCards.Add(cardObject);
        }
    }

    public void UpdateEffectTimers()
    {
        if (defenceTimerText == null || healingTimerText == null)
        {
            Debug.LogError("Timer Text references are not assigned.");//not assigned in unity.
            return;
        }

        defenceTimerText.text = playerBattle.TemporaryDefences.Count == 0
            ? "Current Defence: 0"
            : $"Current Defense: {string.Join(" ", playerBattle.TemporaryDefences.ConvertAll(d => d.value.ToString()))}";

        healingTimerText.text = playerBattle.TemporaryHeals.Count == 0
            ? "Current Healing: 0"
            : $"Current Healing: {string.Join(" ", playerBattle.TemporaryHeals.ConvertAll(h => h.value.ToString()))}";

        defenceTimerText.gameObject.SetActive(true);
        healingTimerText.gameObject.SetActive(true);
    }

    public void AddBattleLog(string message)
    {
        if (activeLogEntries.Count >= MAX_LOG_ENTRIES)
        {
            TextMeshProUGUI oldestLog = activeLogEntries[activeLogEntries.Count - 1];
            activeLogEntries.RemoveAt(activeLogEntries.Count - 1);
            oldestLog.gameObject.SetActive(false);
            logPool.Enqueue(oldestLog);
        }

        TextMeshProUGUI newLogEntry = GetLogEntry();
        newLogEntry.text = message;
        newLogEntry.transform.SetAsFirstSibling();
        activeLogEntries.Insert(0, newLogEntry);

        UpdateLogPositions();
        StartCoroutine(ScrollToTop());
    }

    private TextMeshProUGUI GetLogEntry()
    {
        TextMeshProUGUI entry;
        if (logPool.Count > 0)
        {
            entry = logPool.Dequeue();
        }
        else
        {
            entry = Instantiate(logEntryTemplate, logContainer);
            entry.fontSize = logEntryTemplate.fontSize;
            entry.fontStyle = logEntryTemplate.fontStyle;
            entry.color = logEntryTemplate.color;
            entry.alignment = logEntryTemplate.alignment;
        }
        entry.gameObject.SetActive(true);
        return entry;
    }

    private void UpdateLogPositions()
    {
        float currentY = 0;
        foreach (TextMeshProUGUI entry in activeLogEntries)
        {
            RectTransform rt = entry.rectTransform;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, currentY);
            currentY -= rt.rect.height + logEntrySpacing;
        }

        Vector2 contentSize = logContainer.sizeDelta;
        contentSize.y = Mathf.Abs(currentY);
        logContainer.sizeDelta = contentSize;
    }

    // Set the active enemy for the battle based on the level
    public void SetActiveEnemy(EnemyBattle enemy)
    {
        if (enemy == null)
        {
            Debug.LogError("Enemy reference is null!");
            return;
        }

        enemyBattle = enemy; // Dynamically assign the active enemy
        Debug.Log($"Active enemy set: {enemyBattle.name}");
    }

    private IEnumerator ScrollToTop()
    {
        yield return new WaitForEndOfFrame();
        battleLogScrollRect.verticalNormalizedPosition = 1f;
    }


    public void UpdateTurnIndicator(bool isPlayerTurn)
    {
        if (turnIndicatorText != null && turnIndicatorBackground != null)
        {
            turnIndicatorText.text = isPlayerTurn ? "Player's Turn" : "Enemy's Turn";
            turnIndicatorBackground.color = isPlayerTurn ? playerTurnColor : enemyTurnColor;

            // Optional: Start pulse animation
            StartCoroutine(PulseTurnIndicator());
        }
    }

    private IEnumerator PulseTurnIndicator()
    {
        Vector3 originalScale = turnIndicatorBackground.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;
        float duration = 0.2f;
        float elapsed = 0;

        // Scale up
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            turnIndicatorBackground.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // Scale down
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            turnIndicatorBackground.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        turnIndicatorBackground.transform.localScale = originalScale;
    }

    public IEnumerator ShowCountdown()
    {
        if (gameStatusText == null)
        {
            Debug.LogError("GameStatusText reference missing!");
            yield break;
        }

        gameStatusText.gameObject.SetActive(true);

        // Prepare message
        gameStatusText.text = "Prepare for Battle!";
        gameStatusText.color = Color.white;
        yield return new WaitForSeconds(2f);

        // Begin countdown
        gameStatusText.text = "Begin in...";
        yield return new WaitForSeconds(1f);

        // Numbers countdown
        for (int i = 3; i > 0; i--)
        {
            gameStatusText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        // Fight message
        gameStatusText.text = "Fight!";
        yield return new WaitForSeconds(1f);

        // Then enable battle canvas
        battleCanvas.gameObject.SetActive(true);

        gameStatusText.gameObject.SetActive(false);   
    }

    public IEnumerator ShowBattleResult(bool playerWon)
    {
        if (gameStatusText == null)
        {
            Debug.LogError("GameStatusText reference missing!");
            yield break;
        }

        turnIndicatorText.text = "Game Over";
        turnIndicatorBackground.color = enemyTurnColor;
        DisableCardInteractions();//So the player cannot use cards after the battle is over.

        gameStatusText.gameObject.SetActive(true);
        gameStatusText.text = playerWon ? "Victory!" : "Defeat!";
        gameStatusText.color = Color.white;

        yield return new WaitForSeconds(2f);

        float fadeDuration = 0.5f;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            gameStatusText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        gameStatusText.gameObject.SetActive(false);
    }

    public void DisableCardInteractions()
    {
        battleCanvas.enabled = true;
        if (cardPanel != null)
        {
            cardPanel.SetActive(false);
            Debug.Log("Card interactions disabled");
        }
        else
        {
            Debug.LogError("CardPanel reference missing - cannot disable interactions");
        }
    }

    public void EnableCardInteractions()
    {
        battleCanvas.enabled = true;
        if (cardPanel != null)
        {
            cardPanel.SetActive(true);
            RenderCards(playerBattle.CardLoadout); // Refresh cards
            Debug.Log("Card interactions enabled");
        }
        else
        {
            Debug.LogError("CardPanel reference missing - cannot enable interactions");
        }
    }
}