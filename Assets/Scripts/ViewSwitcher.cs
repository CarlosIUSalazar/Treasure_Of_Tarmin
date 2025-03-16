using UnityEngine;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private Canvas gameCanvas;       // Assign your Game Canvas
    [SerializeField] private Canvas mapAndArmorCanvas; // Assign your Map/Armor Canvas
    
    private void Start()
    {
        // Enable the map camera GameObject if disabled in the Editor
        if (!mapCamera.gameObject.activeSelf)
        {
            mapCamera.gameObject.SetActive(true);
            mapCamera.enabled = false; // Ensure it starts disabled
        }

        // Set initial states
        gameCanvas.enabled = true;
        mapAndArmorCanvas.enabled = false;
        mapCamera.enabled = false;
    }

    public void SwitchToMapAndArmorView()
    {
        // Enable Map Canvas and Map Camera
        mapAndArmorCanvas.enabled = true;
        mapCamera.enabled = true;

        // Disable Game Canvas
        gameCanvas.enabled = false;
    }

    public void SwitchToGameView()
    {
        // Enable Game Canvas
        gameCanvas.enabled = true;

        // Disable Map Canvas and Map Camera
        mapAndArmorCanvas.enabled = false;
        mapCamera.enabled = false;
    }
}
