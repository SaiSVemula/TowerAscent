using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// if playertouches an item then disable the item (used for item drops)
public class ItemCollision : MonoBehaviour 
{ 
    private void OnCollisionEnter(Collision Touches) 
    { 
        if (Touches.gameObject.CompareTag("Player")) 
        { 
            gameObject.SetActive(false); 
        } 
    } 
}
