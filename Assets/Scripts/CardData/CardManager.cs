using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardContainer;

    private void Start()
    {
        // Load all CardData assets from the Resources folder
        CardData[] cards = Resources.LoadAll<CardData>("Cards");
        foreach (CardData card in cards)
        {
            GameObject cardInstance = Instantiate(cardPrefab, cardContainer);
            CardDisplay display = cardInstance.GetComponent<CardDisplay>();
            display.cardData = card;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
