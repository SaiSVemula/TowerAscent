using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    private int cardIndex;
    private BattleManager battleManager;

    // Initialize card UI with the provided card data.
    public void Initialize(Card card, int index, BattleManager manager)
    {
        cardIndex = index;
        battleManager = manager;
    }

    // Called when the card is clicked.
    public void OnCardClicked()
    {
        if (battleManager != null)
        {
            battleManager.OnPlayerUseCard(cardIndex);
        }
    }
}
