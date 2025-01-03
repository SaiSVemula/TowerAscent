using UnityEngine;

public class BarrierCollision : MonoBehaviour
{
    private GeneralNPC npcScript;

    private void Start()
    {
        npcScript = FindObjectOfType<GeneralNPC>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && npcScript != null)
        {
            npcScript.HandleBarrierCollision();
        }
    }
}
