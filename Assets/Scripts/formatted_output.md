### **Battle\BattleEntity.cs**

```csharp

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BattleEntity : MonoBehaviour
{
    protected Animator animator;

    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    [SerializeField] protected int baseDefence = 0;
    protected int currentDefence;

    protected List<Card> cardLoadout = new List<Card>();
    protected List<(int value, int timer)> temporaryDefenses = new List<(int, int)>();
    protected List<(int value, int timer)> temporaryHeals = new List<(int, int)>();

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public int CurrentDefence => currentDefence;
    public List<(int value, int timer)> TemporaryDefences => temporaryDefenses;
    public List<(int value, int timer)> TemporaryHeals => temporaryHeals;
    public List<Card> CardLoadout => cardLoadout;

    protected virtual void Awake()
    {
        animator.SetBool("InBattle", true);
        currentHealth = maxHealth;
        currentDefence = baseDefence;
    }

    public abstract void UseCard(int cardIndex, BattleEntity target);

    public void TakeDamage(int damageAmount)
    {
        int netDamage = Mathf.Max(damageAmount - currentDefence, 0);
        currentHealth = Mathf.Max(currentHealth - netDamage, 0);
        animator.SetTrigger("GetHit");
        Debug.Log($"{name} takes {netDamage} damage. Current health: {currentHealth}");
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"{name} heals {healAmount} HP. Current health: {currentHealth}");
    }

    public void AddDefence(int defenceAmount)
    {
        currentDefence += defenceAmount;
        Debug.Log($"{name}'s defence increased by {defenceAmount}. Current defence: {currentDefence}");
    }

    public void ResetDefence()
    {
        currentDefence = baseDefence;
        Debug.Log($"{name}'s defence reset to base.");
    }

    public void AddTemporaryDefence(int value, int timer)
    {
        temporaryDefenses.Add((value, timer));
        UpdateCurrentDefence();
    }

    public void AddTemporaryHealing(int value, int timer)
    {
        temporaryHeals.Add((value, timer));
    }

    protected void UpdateCurrentDefence()
    {
        currentDefence = baseDefence + temporaryDefenses.Sum(d => d.value);
    }

    public void DecrementEffectTimers()
    {
        // Decrement defense timers
        for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
        {
            temporaryDefenses[i] = (temporaryDefenses[i].value, temporaryDefenses[i].timer - 1);
            if (temporaryDefenses[i].timer <= 0)
            {
                temporaryDefenses.RemoveAt(i);
            }
        }
        UpdateCurrentDefence();

        // Decrement healing timers
        for (int i = temporaryHeals.Count - 1; i >= 0; i--)
        {
            Heal(temporaryHeals[i].value);
            temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
            if (temporaryHeals[i].timer <= 0)
            {
                temporaryHeals.RemoveAt(i);
            }
        }
    }
}


```

---

### **Battle\BattleManager.cs**

```csharp

using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private PlayerBattle playerBattle;
    private EnemyBattle activeEnemy; // The actual enemy used in battle
    [SerializeField] private Transform enemySpawnPoint; // Assign the position where the enemy will spawn
    [SerializeField] private Enemy1 enemy1Prefab;
    [SerializeField] private Enemy2 enemy2Prefab;
    [SerializeField] private Enemy3 enemy3Prefab;
    [SerializeField] private BattleUI battleUI;

    private bool isPlayerTurn = true;

    private LevelLoader levelLoader;

    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();

        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
        StartCoroutine(InitializeBattle());
    }

    public IEnumerator InitializeBattle()
    {
        if (battleUI == null || playerBattle == null)
        {
            Debug.LogError("Critical references missing!");
            yield break;
        }

        Debug.Log("Battle initialization started");

        // Spawn the correct enemy based on the PreviousScene
        SpawnCorrectEnemy();

        if (activeEnemy == null)
        {
            Debug.LogError("Enemy spawning failed!");
            yield break;
        }

        yield return StartCoroutine(battleUI.ShowCountdown());

        Debug.Log("Battle initialization in progress");

        battleUI.RenderCards(playerBattle.CardLoadout);
        battleUI.EnableCardInteractions();
        battleUI.AddBattleLog($"Battle Start! Enemy: {activeEnemy.EnemyName} with {activeEnemy.CurrentHealth} HP");
        battleUI.UpdateTurnIndicator(true);

        Debug.Log("Battle initialization complete");
    }

    private void SpawnCorrectEnemy()
    {
        string previousScene = GameManager.Instance.PreviousScene;

        Debug.Log($"Spawning enemy based on previous scene: {previousScene}");

        switch (previousScene)
        {
            case "ExplorationScene":
                activeEnemy = Instantiate(enemy1Prefab, enemySpawnPoint.position, Quaternion.identity);
                break;
            case "Level 1":
                activeEnemy = Instantiate(enemy2Prefab, enemySpawnPoint.position, Quaternion.identity);
                break;
            case "Level 2":
                activeEnemy = Instantiate(enemy3Prefab, enemySpawnPoint.position, Quaternion.identity);
                break;
            default:
                Debug.LogError("Invalid PreviousScene value!");
                return;
        }

        if (activeEnemy != null)
        {
            activeEnemy.Initialize(GameManager.Instance.GameDifficulty);
        }
        else
        {
            Debug.LogError("Failed to spawn enemy!");
        }
    }
    public void OnPlayerUseCard(int cardIndex)
    {
        if (!isPlayerTurn)
        {
            return;
        }

        if (cardIndex < 0 || cardIndex >= playerBattle.CardLoadout.Count)
        {
            Debug.LogError("Invalid card index!");
            return;
        }

        Card selectedCard = playerBattle.CardLoadout[cardIndex];
        if (selectedCard == null)
        {
            Debug.LogError("Selected card is null!");
            return;
        }

        string logMessage = selectedCard.Use(playerBattle, activeEnemy);
        battleUI.AddBattleLog(logMessage);

        if (activeEnemy.CurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(true));
            return;
        }

        battleUI.UpdateEffectTimers();
        isPlayerTurn = false;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.DisableCardInteractions();
        StartCoroutine(EnemyTurnWithDelay());
    }

    private IEnumerator EnemyTurnWithDelay()
    {
        yield return new WaitForSeconds(1f); // Add a delay before enemy action

        Debug.Log($"{activeEnemy.EnemyName} attacks the player!");
        playerBattle.TakeDamage(10); // Placeholder damage for the attack

        // Check if player is defeated
        if (playerBattle.CurrentHealth <= 0)
        {
            StartCoroutine(EndBattle(false));
            yield break;
        }

        // Update player effects and UI for the next turn
        playerBattle.DecrementEffectTimers();
        battleUI.UpdateEffectTimers();
        isPlayerTurn = true;
        battleUI.UpdateTurnIndicator(isPlayerTurn);
        battleUI.RenderCards(playerBattle.CardLoadout);
        battleUI.EnableCardInteractions();
    }


    public IEnumerator EndBattle(bool playerWon)
    {
        Debug.Log(playerWon ? "Player wins!" : "Enemy wins!");
        yield return StartCoroutine(battleUI.ShowBattleResult(playerWon));

        isPlayerTurn = false;

        // Transition logic (e.g., load next scene or return to menu)
        Debug.Log("Transitioning to next scene...");
        string nextScene = GameManager.Instance.NextScene;
        levelLoader.LoadScene("BattleScene", nextScene);
    }
}


```

---

### **Battle\BattleUI.cs**

```csharp

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

    // Start battle button click event
    private void OnStartBattleClick()
    {
        if (!isInitialized)
        {
            Debug.LogError("BattleUI not initialized!");
            return;
        }

        Debug.Log("Start Battle clicked");

        // First disable instruction canvas
        instructionCanvas.enabled = false;

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

```

---

### **Battle\TriggerZoneHandler.cs**

```csharp

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TriggerZoneHandler : MonoBehaviour
{
    public GameObject promptPanel;
    public GameObject EnemyPill;
    public TMP_Text promptMessage;
    public Button yesButton;
    public Button noButton;
    public Transform player;
    public Vector3 outsideBoxPosition;

    private bool isPromptActive = false;
    private bool isLoading = false;
    private bool isTriggered = false;

    private LevelLoader levelLoader;

    void Start()
    {
        // Find the LevelLoader instance in the current scene
        levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader == null)
        {
            Debug.LogError("LevelLoader prefab not found in the scene. Make sure it is added as a prefab to the scene.");
        }
        // Hide the prompt panel and add listeners
        promptPanel.SetActive(false);
        yesButton.onClick.AddListener(OnYesButton);
        noButton.onClick.AddListener(OnNoButton);
    }

    // Function to handle the trigger enter event
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameObject.CompareTag("PromptZone"))
        {
            ShowPrompt();
            EnemyPill.SetActive(true);
        }
        else if (gameObject.CompareTag("ForcedZone") && !isLoading)
        {
            isTriggered = true;
            StartCoroutine(StartLoadingAnimation());
        }
    }

    // Function to show the prompt panel
    public void ShowPrompt()
    {
        if (!isTriggered)
        {
            promptPanel.SetActive(true);
            promptMessage.SetText("Do you wish to enter battle?");
            isPromptActive = true;

            // Make sure the buttons are visible
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
        }
    }

    // Function to handle the "Yes" button click
    public void OnYesButton()
    {
        if (isPromptActive)
        {
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            // Start the loading sequence
            StartCoroutine(StartLoadingAnimation());
        }
    }

    // Function to handle the "No" button click
    public void OnNoButton()
    {
        if (isPromptActive)
        {
            // Hide both buttons
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            promptPanel.SetActive(false);
            player.position = outsideBoxPosition;
            isPromptActive = false;
        }
    }

    // Coroutine to animate the loading process and use the LevelLoader
    IEnumerator StartLoadingAnimation()
    {
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);

        // Set the loading state to true
        isLoading = true;
        promptMessage.SetText("Loading");
        promptMessage.alignment = TextAlignmentOptions.Center;

        promptPanel.GetComponent<Image>().color = Color.red;

        string[] loadingStates = { "Loading", "Loading.", "Loading..", "Loading..." };
        int index = 0;

        // Loop through the loading animation for 5 seconds
        for (int i = 0; i < 5; i++)
        {
            promptMessage.SetText(loadingStates[index]);
            index = (index + 1) % loadingStates.Length;
            yield return new WaitForSeconds(1f);
        }

        // Set the loading state to false
        isTriggered = false;

        //getting scene names to set which scene to transition to after battle.
        string nextScene;
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "ExplorationScene")
        {
            nextScene = "Level 1";
        }
        else if (currentScene == "Level 1")
        {
            nextScene = "Level 2";
        }
        else
        {
            nextScene = "EndPage";
        }

        // Use the LevelLoader to load the scene
        if (levelLoader != null)
        {
            GameManager.Instance.NextScene = nextScene;
            Debug.Log($"Next scene set to: {nextScene}");
            levelLoader.LoadScene(currentScene, "Loadout");
        }
        else
        {
            Debug.LogError("LevelLoader instance not found. Ensure LevelLoader is in the scene.");
        }
    }
}


```

---

### **Card\Card.cs**

```csharp

using UnityEngine;

public enum CardType
{
    Common,
    Rare,
    Epic,
    Legendary
}

//abstract class for all cards to inherit 

public abstract class Card : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private CardType type;
    [SerializeField] private Sprite cardSprite;

    public string Name => name;
    public string Description => description;
    public CardType Type => type;
    public Sprite CardSprite => cardSprite;

    // Generalized Use method for both player and enemy
    public abstract string Use(BattleEntity user, BattleEntity target);
}


```

---

### **Card\CardDisplay.cs**

```csharp

using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card CardData { get; private set; } // Reference to the Card scriptable object

    [SerializeField] private Image cardImage; // UI Image to display the card's sprite
    [SerializeField] private Text cardNameText; // UI Text to display the card's name

    // Initialize card UI with the provided card data
    public void Initialize(Card card)
    {
        CardData = card;

        // Update UI elements
        if (cardImage != null)
        {
            cardImage.sprite = card.CardSprite; // Use the card's sprite
        }

        if (cardNameText != null)
        {
            cardNameText.text = card.Name; // Display the card's name
        }
    }
}


```

---

### **Card\DefenceCard.cs**

```csharp

using UnityEngine;

// Defense card class works like a temporary armmor for the player
// best to play when you have low health and companion can attack for you

[CreateAssetMenu(menuName = "Card/DefenceCard")]
public class DefenceCard : Card
{
    [SerializeField] private int defence;
    [SerializeField] private int timer;

    public int Defence => defence;
    public int Timer => timer;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        user.AddTemporaryDefence(Defence, Timer);
        return $"{user.name} used {Name}, gaining {Defence} defense for {Timer} turns!";
    }
}


```

---

### **Card\HealingCard.cs**

```csharp

using UnityEngine;

[CreateAssetMenu(menuName = "Card/HealingCard")]
public class HealingCard : Card
{
    [SerializeField] private int healing;
    [SerializeField] private int timer;
    [SerializeField] private bool canRevive;//not added in yet 
    public int Healing => healing;
    public int Timer => timer;
    public bool CanRevive => canRevive;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        if (CanRevive)
        {
            return $"{user.name} used {Name} attempting to revive!";
        }

        user.AddTemporaryHealing(Healing, Timer);
        return $"{user.name} used {Name}, healing {Healing} HP for {Timer} turns!";
    }
}


```

---

### **Card\MagicCard.cs**

```csharp

using UnityEngine;

[CreateAssetMenu(menuName = "Card/MagicCard")]
public class MagicCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        target.TakeDamage(damage);
        return $"{user.name} cast {Name}, dealing {damage} magic damage to {target.name}!";
    }
}

```

---

### **Card\WeaponCard.cs**

```csharp

using UnityEngine;

[CreateAssetMenu(menuName = "Card/WeaponCard")]
public class WeaponCard : Card
{
    [SerializeField] private int damage;

    public int Damage => damage;

    public override string Use(BattleEntity user, BattleEntity target)
    {
        target.TakeDamage(damage);
        return $"{user.name} used {Name}, dealing {damage} damage to {target.name}!";
    }
}


```

---

### **Core\GameManager.cs**

```csharp

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance of GameManager
    public static GameManager Instance;

    // Reference to the Player
    private Player playerInstance;

    // Game state variables

    // Scene management
    private string SavedScene;
    private string previousScene;
    private string nextScene;
    public string PreviousScene { get => previousScene; set => previousScene = value; }
    public string NextScene { get => nextScene; set => nextScene = value; }

    // Difficulty
    public Difficulty GameDifficulty { get; set; }

    private Vector3 PlayerCoord;
    private int CurrentHealth;
    private int CurrentCoins;
    private string[] CardsInInventory;
    private string PlayerName;

    private int minibattleWins;
    private int minibattleLosses;
    private int bigbattleWins;
    private int bigbattleLosses;

    private void Awake()
    {
        // Singleton pattern for GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist GameManager across scenes
        }
    }

    // Initialize or retrieve the Player instance
    public Player GetPlayer()
    {
        if (playerInstance == null)
        {
            // Try to find an existing Player object in the scene
            playerInstance = FindObjectOfType<Player>();

            if (playerInstance == null)
            {
                // If no Player exists, create a new Player GameObject
                GameObject playerObject = new GameObject("Player");
                playerInstance = playerObject.AddComponent<Player>();

                // Set default Player values
                playerInstance.PlayerName = "Hero";
                playerInstance.Gold = 100;
                playerInstance.CurrentScene = SceneManager.GetActiveScene().name;
                playerInstance.CurrentHealth = 100; // Default health
            }

            DontDestroyOnLoad(playerInstance.gameObject); // Make the Player persistent
        }

        return playerInstance;
    }

    // Save the Player's state
    public void SavePlayerState()
    {
        if (playerInstance != null)
        {
            PlayerCoord = playerInstance.transform.position;
            CurrentHealth = playerInstance.CurrentHealth;
            CurrentCoins = playerInstance.Gold;
            CardsInInventory = playerInstance.Inventory.ToArray();
            PlayerName = playerInstance.PlayerName;

            Debug.Log("Player state saved.");
        }
    }

    // Restore the Player's state
    public void LoadPlayerState()
    {
        if (playerInstance != null)
        {
            playerInstance.transform.position = PlayerCoord;
            playerInstance.CurrentHealth = CurrentHealth;
            playerInstance.Gold = CurrentCoins;
            playerInstance.Inventory = new List<string>(CardsInInventory);
            playerInstance.PlayerName = PlayerName;

            Debug.Log("Player state loaded.");
        }
    }

    // Methods to get single variables, for other scripts
    public string GetCurrentScene() { return SavedScene; }
    public Vector3 GetPlayerLocation() { return PlayerCoord; }
    public int GetPlayerHealth() { return CurrentHealth; }
    public int GetPlayerCoinCount() { return CurrentCoins; }
    public string[] GetPlayerCards() { return CardsInInventory; }
    public string GetPlayerName() { return PlayerName; }
    public int GetMinibattleWins() { return minibattleWins; }
    public int GetMinibattleLosses() { return minibattleLosses; }
    public int GetBigbattleWins() { return bigbattleWins; }
    public int GetBigbattleLosses() { return bigbattleLosses; }

    // Methods to set values on the GameManager
    public void UpdateCurrentScene() { SavedScene = SceneManager.GetActiveScene().name; }
    public void UpdatePlayerLocation(Vector3 location) { PlayerCoord = location; }
    public void UpdatePlayerHealth(int health) { CurrentHealth = health; }
    public void UpdatePlayerCoinCount(int coins) { CurrentCoins = coins; }
    public void UpdatePlayerCards(string[] cards) { CardsInInventory = cards; }
    public void UpdatePlayerName(string name) { PlayerName = name; }
    public void UpdateMinibattleWins(int wins) { minibattleWins = wins; }
    public void UpdateMinibattleLosses(int losses) { minibattleLosses = losses; }
    public void UpdateBigbattleWins(int wins) { bigbattleWins = wins; }
    public void UpdateBigbattleLosses(int losses) { bigbattleLosses = losses; }

    // Full get method, used when saving a game to perfs
    public (string, Vector3, int, int, string[], string, int, int, int, int) GetFullGameState()
    {
        return (SavedScene, PlayerCoord, CurrentHealth, CurrentCoins, CardsInInventory, PlayerName, minibattleWins, minibattleLosses, bigbattleWins, bigbattleLosses);
    }

    // Full update method, used when resuming a game and loading from perfs
    public void UpdateFullGameState(Transform playerTransform, int health, int coins, string[] cards, string name, int miniWins, int miniLosses, int bigWins, int bigLosses)
    {
        UpdatePlayerLocation(playerTransform.position);
        UpdatePlayerHealth(health);
        UpdatePlayerCoinCount(coins);
        UpdatePlayerCards(cards);
        UpdatePlayerName(name);
        UpdateMinibattleWins(miniWins);
        UpdateMinibattleLosses(miniLosses);
        UpdateBigbattleWins(bigWins);
        UpdateBigbattleLosses(bigLosses);
        UpdateCurrentScene();
        Debug.Log("Game state updated.");
    }

    // Used when clicking out of a menu mid gameplay
    public void LoadGameState()
    {
        SceneManager.LoadScene(SavedScene);
        if (playerInstance != null)
        {
            playerInstance.transform.position = PlayerCoord;
        }
    }

    // Loads a new scene and saves the current state
    public void LoadScene(string nextScene)
    {
        SavePlayerState();
        SceneManager.LoadScene(nextScene);
    }

    // Reset Game State (resets all variables)
    public void Clear()
    {
        PlayerCoord = Vector3.zero;
        CurrentHealth = 100;
        CurrentCoins = 0;
        CardsInInventory = new string[0];
        SavedScene = "";
        NextScene = "";
        PlayerName = "";

        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
            playerInstance = null;
        }

        Debug.Log("Game state cleared.");
    }
}


```

---

### **Enemy\Enemy1.cs**

```csharp

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy1")]
public class Enemy1 : EnemyBattle
{
    protected override void SetupEnemyStatsAndCards()
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                EnemyName = "Warden of Ash";
                maxHealth = 100;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Dodge"),
                    Resources.Load<Card>("Cards/First Aid")
                };
                break;
            case Difficulty.Medium:
                EnemyName = "Warden of Cinders";
                maxHealth = 70;
                cardLoadout = new List<Card> { Resources.Load<Card>("Cards/Axe Chop") };
                break;
            case Difficulty.Easy:
                EnemyName = "Warden of Infernos";
                maxHealth = 50;
                cardLoadout = new List<Card> { Resources.Load<Card>("Cards/Dagger Slash") };
                break;
        }
    }
}


```

---

### **Enemy\Enemy2.cs**

```csharp

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy2")]
public class Enemy2 : EnemyBattle
{
    protected override void SetupEnemyStatsAndCards()
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                EnemyName = "Dusk Herald";
                maxHealth = 200;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Hammer Smash"),
                    Resources.Load<Card>("Cards/Magic Block"),
                    Resources.Load<Card>("Cards/Healing Potion")
                };
                break;
            case Difficulty.Medium:
                EnemyName = "Twilight Regent";
                maxHealth = 150;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Spear Thrust"),
                    Resources.Load<Card>("Cards/Reflect")
                };
                break;
            case Difficulty.Easy:
                EnemyName = "Midnight Emperor";
                maxHealth = 100;
                cardLoadout = new List<Card> { Resources.Load<Card>("Cards/Spear Thrust") };
                break;
        }
    }
}


```

---

### **Enemy\Enemy3.cs**

```csharp

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy3")]
public class Enemy3 : EnemyBattle
{
    protected override void SetupEnemyStatsAndCards()
    {
        switch (difficulty)
        {
            case Difficulty.Hard:
                EnemyName = "The Eternal Verdict";
                maxHealth = 300;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Meteor Shower")
                };
                // Companion logic can be added here.
                break;
            case Difficulty.Medium:
                EnemyName = "The Shadowed Adjudicator";
                maxHealth = 250;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Group Heal"),
                    Resources.Load<Card>("Cards/Magic Barrier"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
            case Difficulty.Easy:
                EnemyName = "The Silent Judge";
                maxHealth = 150;
                cardLoadout = new List<Card>
                {
                    Resources.Load<Card>("Cards/Sword Slash"),
                    Resources.Load<Card>("Cards/Earthquake")
                };
                break;
        }
    }
}


```

---

### **Enemy\EnemyBattle.cs**

```csharp

using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBattle : BattleEntity
{
    [SerializeField] private List<Card> cardLoadoutForDifficulty = new List<Card>();
    public string EnemyName { get; protected set; }

    protected Difficulty difficulty;

    public void Initialize(Difficulty gameDifficulty)
    {
        difficulty = gameDifficulty;
        SetupEnemyStatsAndCards();
        currentHealth = maxHealth; // Ensure health is set correctly
    }

    protected abstract void SetupEnemyStatsAndCards();

    public override void UseCard(int cardIndex, BattleEntity target)
    {
        if (cardIndex < 0 || cardIndex >= cardLoadout.Count)
        {
            return;
        }

        Card selectedCard = cardLoadout[cardIndex];
        if (selectedCard != null)
        {
            Debug.Log($"{EnemyName} used {selectedCard.Name} on {target.name}");
            Debug.Log(selectedCard.Use(this, target));
        }
    }
}


```

---

### **Enemy\IEnemy.cs**

```csharp

public interface IEnemy
{
    string EnemyName { get; }
    int EnemyCurrentHealth { get; }
    void Initialize(Difficulty difficulty);
    void TakeDamage(int damageAmount);
    void AttackPlayer(PlayerBattle player);
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public enum EnemyLevel
{
    Level1,
    Level2,
    Level3
}


```

---

### **Player\Player.cs**

```csharp

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton instance of Player
    public static Player Instance;

    // Player Information
    public string PlayerName;
    public string CurrentScene;
    public string CurrentCheckpoint;
    public int Gold;
    public int MiniBattleWins;
    public int MiniBattleLosses;
    public int BigBattleWins;
    public int BigBattleLosses;
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;


    // Inventory (placeholder for future implementation)
    public List<string> Inventory = new List<string>();

    // Objectives: Key-value pairs for objective names and their completion status
    public Dictionary<string, bool> Objectives = new Dictionary<string, bool>();

    // Calculate Total Battles
    public int TotalMiniBattles => MiniBattleWins + MiniBattleLosses;
    public int TotalBigBattles => BigBattleWins + BigBattleLosses;

    // Getters and setters for health
    public int CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, maxHealth); }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Calculate Mini Battle Win/Loss Ratio
    public float GetMiniBattleWinLossRatio()
    {
        if (TotalMiniBattles > 0)
        {
            float ratio = (float)MiniBattleWins / TotalMiniBattles;
            Debug.Log($"Mini Battle Win/Loss Ratio: {ratio:F2}");
            return ratio;
        }

        Debug.Log("No mini battles fought. Win/Loss Ratio is 0.");
        return 0;
    }

    // Calculate Big Battle Win/Loss Ratio
    public float GetBigBattleWinLossRatio()
    {
        if (TotalBigBattles > 0)
        {
            float ratio = (float)BigBattleWins / TotalBigBattles;
            Debug.Log($"Big Battle Win/Loss Ratio: {ratio:F2}");
            return ratio;
        }

        Debug.Log("No big battles fought. Win/Loss Ratio is 0.");
        return 0;
    }

    // Add win/loss methods
    public void AddMiniBattleWin()
    {
        MiniBattleWins++;
        Debug.Log($"Mini battle won! Total wins: {MiniBattleWins}");
    }

    public void AddMiniBattleLoss()
    {
        MiniBattleLosses++;
        Debug.Log($"Mini battle lost! Total losses: {MiniBattleLosses}");
    }

    public void AddBigBattleWin()
    {
        BigBattleWins++;
        Debug.Log($"Big battle won! Total wins: {BigBattleWins}");
    }

    public void AddBigBattleLoss()
    {
        BigBattleLosses++;
        Debug.Log($"Big battle lost! Total losses: {BigBattleLosses}");
    }

    // Update gold
    public void AddGold(int amount)
    {
        Gold += amount;
        Debug.Log($"Added {amount} gold. Total gold: {Gold}");
    }

    public void DeductGold(int amount)
    {
        Gold = Mathf.Max(0, Gold - amount);
        Debug.Log($"Deducted {amount} gold. Total gold: {Gold}");
    }

    // Objective management
    public void CompleteObjective(string objectiveName)
    {
        if (Objectives.ContainsKey(objectiveName))
        {
            Objectives[objectiveName] = true;
            Debug.Log($"Objective '{objectiveName}' marked as completed.");
        }
        else
        {
            Objectives.Add(objectiveName, true);
            Debug.Log($"New objective '{objectiveName}' added and marked as completed.");
        }
    }

    public bool IsObjectiveCompleted(string objectiveName)
    {
        return Objectives.ContainsKey(objectiveName) && Objectives[objectiveName];
    }

    // Debugging
    public void DebugStats()
    {
        Debug.Log($"Player: {PlayerName}, Gold: {Gold}");
        Debug.Log($"Mini Battles: Wins={MiniBattleWins}, Losses={MiniBattleLosses}, Total={TotalMiniBattles}, Win/Loss Ratio={GetMiniBattleWinLossRatio():F2}");
        Debug.Log($"Big Battles: Wins={BigBattleWins}, Losses={BigBattleLosses}, Total={TotalBigBattles}, Win/Loss Ratio={GetBigBattleWinLossRatio():F2}");
        Debug.Log($"Objectives: {string.Join(", ", Objectives.Keys)}");
    }
}


```

---

### **Player\PlayerBattle.cs**

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : BattleEntity
{
    [SerializeField] private Slider healthBar;
    protected override void Awake()
    {
        base.Awake();
        healthBar.maxValue = maxHealth;
        UpdateHealthBar();

        // Initialize card loadout
        cardLoadout = new List<Card>
        {
            Resources.Load<Card>("Cards/Axe Chop"),
            Resources.Load<Card>("Cards/Fireball"),
            Resources.Load<Card>("Cards/Dodge"),
            Resources.Load<Card>("Cards/First Aid")
        };
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    public override void UseCard(int cardIndex, BattleEntity target)
    {
        if (cardIndex < 0 || cardIndex >= cardLoadout.Count)
        {
            return;
        }

        Card selectedCard = cardLoadout[cardIndex];
        if (selectedCard != null)
        {
            Debug.Log(selectedCard.Use(this, target));
            animator.SetTrigger("Attack");
        }
    }

    public void UpdateEffectTimers()
    {
        // Implement timer decrement logic for temporary effects
    }


    // Decrement all effect timers and remove expired effects
    public void DecrementEffectTimers()
    {
        // Defense timers
        for (int i = temporaryDefenses.Count - 1; i >= 0; i--)
        {
            temporaryDefenses[i] = (temporaryDefenses[i].value, temporaryDefenses[i].timer - 1);
            if (temporaryDefenses[i].timer <= 0)
            {
                temporaryDefenses.RemoveAt(i);
            }
        }
        UpdateCurrentDefence();

        // Healing timers
        for (int i = temporaryHeals.Count - 1; i >= 0; i--)
        {
            // Apply healing before decrementing timer
            Heal(temporaryHeals[i].value); // Replace PlayerHeal with Heal
            temporaryHeals[i] = (temporaryHeals[i].value, temporaryHeals[i].timer - 1);
            if (temporaryHeals[i].timer <= 0)
            {
                temporaryHeals.RemoveAt(i);
            }
        }
    }
}



```

---