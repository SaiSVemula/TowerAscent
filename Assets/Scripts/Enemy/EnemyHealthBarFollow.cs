using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarFollow : MonoBehaviour
{
    private Camera mainCamera;
    private Transform enemyTransform;

    void Start()
    {
        // Find the camera and the enemy transform
        mainCamera = Camera.main;
        enemyTransform = transform.parent; // Assuming the health bar canvas is a child of the enemy
    }

    void Update()
    {
        // Update the position of the health bar to follow the enemy
        if (mainCamera != null && enemyTransform != null)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(enemyTransform.position + Vector3.up * 2.0f);
            transform.position = screenPosition;
        }
    }
}
