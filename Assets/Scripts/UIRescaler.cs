using UnityEngine;

public class UIRescaler : MonoBehaviour
{
    public RectTransform topUI; // Assign the top UI group
    public RectTransform bottomUI; // Assign the bottom UI group

    void Start()
    {
        AdjustUI();
        Debug.Log("UI Rescaled");
    }

    void AdjustUI()
    {
        // Get screen dimensions
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        // Adjust top UI
        topUI.offsetMin = new Vector2(0, screenHeight * 0.9f); // Top 10% padding
        topUI.offsetMax = new Vector2(0, 0);

        // Adjust bottom UI
        bottomUI.offsetMin = new Vector2(0, 0);
        bottomUI.offsetMax = new Vector2(0, screenHeight * 0.1f); // Bottom 10% padding

    }
}
