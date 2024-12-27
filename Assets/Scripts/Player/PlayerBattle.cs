using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBattle : MonoBehaviour
{
    [SerializeField] private int playerMaxHealth = 100;
    private int playerCurrentHealth;

    [SerializeField] private int playerBaseDefence = 0;
    private int playerCurrentDefence;

    [SerializeField] private List<Card> playerCardLoadout;

    [SerializeField] private Slider playerHealthBar; // Player-specific health bar
    public int PlayerCurrentHealth => playerCurrentHealth;
    public int PlayerCurrentDefence => playerCurrentDefence;
    public List<Card> PlayerCardLoadout => playerCardLoadout;



    private void Awake()
    {
        playerCurrentHealth = playerMaxHealth;
        playerCurrentDefence = playerBaseDefence;

        playerHealthBar.maxValue = playerMaxHealth;
        UpdatePlayerHealthBar();

        playerCardLoadout = new List<Card>
        {
            Resources.Load<Card>("Cards/Axe Chop"),
            Resources.Load<Card>("Cards/Fireball"),
            Resources.Load<Card>("Cards/Dodge"),
            Resources.Load<Card>("Cards/First Aid")
        };

        

    }

    public void UpdatePlayerHealthBar()
    {
        if (playerHealthBar != null)
        {
            playerHealthBar.value = playerCurrentHealth;
        }
    }

    public void PlayerTakeDamage(int damageAmount)
    {
        int netDamage = Mathf.Max(damageAmount - playerCurrentDefence, 0);
        playerCurrentHealth = Mathf.Max(playerCurrentHealth - netDamage, 0);
        Debug.Log($"Player takes {netDamage} damage. Current health: {playerCurrentHealth}");
    }

    public void PlayerAddDefence(int defenceAmount)
    {
        playerCurrentDefence += defenceAmount;
        Debug.Log($"Player defence increased by {defenceAmount}. Current defence: {playerCurrentDefence}");
    }

    public void PlayerHeal(int healAmount)
    {
        playerCurrentHealth = Mathf.Min(playerCurrentHealth + healAmount, playerMaxHealth);
        Debug.Log($"Player heals {healAmount}. Current health: {playerCurrentHealth}");
    }

    public void PlayerResetDefence()
    {
        playerCurrentDefence = playerBaseDefence;
        Debug.Log("Player defence reset to base.");
    }

    public void UsePlayerCard(int cardIndex, EnemyBattle targetEnemy)
    {
        if (cardIndex < 0 || cardIndex >= playerCardLoadout.Count) return;

        Card selectedCard = playerCardLoadout[cardIndex];
        if (selectedCard != null)
        {
            selectedCard.Use(this, targetEnemy);
        }
    }
}
