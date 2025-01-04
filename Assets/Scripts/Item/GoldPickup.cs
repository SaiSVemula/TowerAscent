using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    [SerializeField] private int goldAmount = 10; // Amount of gold this object provides

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Increase the player's gold count in the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdatePlayerCoinCount(GameManager.Instance.GetPlayerCoinCount() + goldAmount);
            }
            else
            {
                Debug.LogError("GameManager instance is missing!");
            }

            // Destroy the gold object after pickup
            Destroy(gameObject);
        }
    }
}
