using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class PlayerTTH : MonoBehaviour
{
    [Header("Text Settings")]
    public TMP_Text playerText; // Reference to the TextMeshPro component
    public float textDisplayDuration = 1.5f; // Duration to display each line

    private void Start()
    {
        // Start the sequence of displaying text
        StartCoroutine(DisplayPlayerThoughts());
    }

    private System.Collections.IEnumerator DisplayPlayerThoughts()
    {
        if (playerText == null)
        {
            Debug.LogError("PlayerText (TextMeshPro) is not assigned in the inspector!");
            yield break;
        }

        // Display the first line
        playerText.text = "You: How did I get here??";
        yield return new WaitForSeconds(textDisplayDuration);

        // Clear the text before displaying the next line
        playerText.text = "";
        yield return new WaitForSeconds(0.5f); // Short delay for clearing

        // Display the second line
        playerText.text = "You: I don't remember anything";
        yield return new WaitForSeconds(textDisplayDuration);

        // Clear the text
        playerText.text = "";
    }
}
