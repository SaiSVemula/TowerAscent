using UnityEngine;
using System;
using System.Collections.Generic;

public class CardLoader : MonoBehaviour
{
    public Card LoadCardByName(string cardName)
    {
        // Define folder paths for each card type
        string weaponPath = "Cards/Weapon Cards/";
        string magicPath = "Cards/Magic Cards/";
        string defencePath = "Cards/Defence Cards/";
        string healingPath = "Cards/Healing Cards/";

        // Array of folder paths for lookup
        string[] paths = { weaponPath, magicPath, defencePath, healingPath };

        foreach (string path in paths)
        {
            // Construct the full path and try loading the card
            string fullPath = path + cardName;
            Card loadedCard = Resources.Load<Card>(fullPath);

            if (loadedCard != null)
            {
                Debug.Log($"Card '{cardName}' loaded from '{fullPath}'.");
                return loadedCard;
            }
        }

        // If card is not found in any folder
        Debug.LogError($"Card '{cardName}' not found in any folder.");
        return null;
    }

    public List<Card> AddCardToList(string[] cardNames)
    {
        List<Card> cardList = new List<Card>();
        
        foreach (string cardName in cardNames)
        {
            Card card = LoadCardByName(cardName);
            if (card != null)
            {
                cardList.Add( card );
            }
            else
            {
                Debug.LogError($"Card '{cardName}' not found in any folder."); 
            }
        }

        return cardList;
    }
}