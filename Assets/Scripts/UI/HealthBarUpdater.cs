using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUpdater : MonoBehaviour
{
    [SerializeField] private Slider playerHealthBar; // Reference to the Slider component
    [SerializeField] private TextMeshProUGUI healthText; // Optional: Display numerical health value (if desired)

    public void Start()
    {
        // Fetch the current player health from GameManager
        int currentHealth = GameManager.Instance.GetPlayerHealth();

        if (playerHealthBar != null)
        {
            playerHealthBar.maxValue = currentHealth;
            playerHealthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
        }
    }


    public void Update()
    {
        // Fetch the current player health from the GameManager
        int currentHealth = GameManager.Instance.GetPlayerHealth();

        // Update the slider value
        playerHealthBar.value = currentHealth;

        // Update the text display (if assigned)
        if (healthText != null)
        {
            healthText.text = $"{currentHealth}";
        }
    }
}
