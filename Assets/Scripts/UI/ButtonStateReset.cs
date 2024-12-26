using UnityEngine;
using UnityEngine.EventSystems; // Make sure this namespace is included

public class ButtonStateReset : MonoBehaviour
{
    public void ResetButtonSelection()
    {
        // Check if an EventSystem exists in the scene
        if (EventSystem.current != null)
        {
            // Deselect the button after it is clicked
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Debug.LogWarning("No EventSystem found in the scene. Ensure you have an EventSystem component in your hierarchy.");
        }
    }
}
