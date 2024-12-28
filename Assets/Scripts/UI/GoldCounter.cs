using UnityEngine;
using TMPro;

public class GoldCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText; // Reference to the GoldText UI element

    private void Start()
    {
        if (goldText == null)
        {
            goldText = GetComponent<TextMeshProUGUI>();
        }

        UpdateGoldText(); // Ensure the text is updated when the game starts
    }

    private void Update()
    {
        UpdateGoldText(); // Continuously update the coin count
    }

    private void UpdateGoldText()
    {
        if (goldText != null && GameManager.Instance != null)
        {
            int currentCoins = GameManager.Instance.GetPlayerCoinCount();
            goldText.text = $"Gold: {currentCoins}";
        }
        else
        {
            Debug.LogError("GoldText reference or GameManager instance is missing!");
        }
    }
}
