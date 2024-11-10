using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code that runs when player is near the companion
public class CompanionTriggerZone : MonoBehaviour
{
    public GameObject floatingText;
    private bool playerNearby = false;

    void Start()
    {
        floatingText.SetActive(false);
    }

    // What to do when player is near
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            floatingText.SetActive(true);
        }
    }

    // What to do when player is not near
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            floatingText.SetActive(false);
        }
    }

    // Constantly check for these conditions and run CompanionAI code if true
    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<CompanionAI>().Befriend();
        }
    }
}
