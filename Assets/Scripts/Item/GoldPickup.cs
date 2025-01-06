using UnityEngine;
using TMPro;

public class GoldPickup : MonoBehaviour
{
    [SerializeField] private int goldAmount = 10; // Amount of gold this object provides
    [SerializeField] private TextMeshProUGUI goldPickupText; // Reference to the shared TextMeshPro component
    [SerializeField] private float textDisplayDuration = 2f; // Duration to display the text

    private void Start()
    {
        if (goldPickupText != null)
        {
            goldPickupText.text = "";
            goldPickupText.gameObject.SetActive(false);
            Debug.Log("GoldPickupText hidden initially.");
        }
        else
        {
            Debug.LogError("GoldPickupText is not assigned in the inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.GetPlayerCoinCount() + goldAmount);
                Debug.Log($"Player's gold updated. New total: {GameManager.Instance.GetPlayerCoinCount()}");
            }
            else
            {
                Debug.LogError("GameManager instance is missing!");
            }

            if (goldPickupText != null)
            {
                ShowGoldPickupMessage();
            }

            // Delay destruction to allow the coroutine to finish
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private void ShowGoldPickupMessage()
    {
        if (goldPickupText != null)
        {
            goldPickupText.text = $"Picked up {goldAmount} gold!";
            goldPickupText.gameObject.SetActive(true);
            Debug.Log("GoldPickupText displayed.");

            StartCoroutine(HideGoldPickupMessage());
        }
    }

    private System.Collections.IEnumerator HideGoldPickupMessage()
    {
        yield return new WaitForSeconds(textDisplayDuration);

        if (goldPickupText != null)
        {
            goldPickupText.text = "";
            goldPickupText.gameObject.SetActive(false);
            Debug.Log("GoldPickupText hidden.");
        }
        else
        {
            Debug.LogWarning("GoldPickupText is null when trying to hide it!");
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(textDisplayDuration);
        Debug.Log("Gold object destroyed.");
        Destroy(gameObject);
    }
}
