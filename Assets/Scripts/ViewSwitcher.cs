using UnityEngine;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private Canvas gameCanvas;       // Assign your Game Canvas
    [SerializeField] private Canvas mapAndArmorCanvas; // Assign your Map/Armor Canvas
    [SerializeField] private GameObject castleFinish;
    [SerializeField] private Button mapBackButton;
    MazeGenerator mazeGenerator;
    PlayerAmbushDetection playerAmbushDetection;
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;


    private void Start()
    {
        mazeGenerator = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerAmbushDetection = GameObject.Find("Player").GetComponent<PlayerAmbushDetection>();
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

    public void GameWinningSequence()
    {
        mazeGenerator.currentPlayerBlock.playerCursor.transform.localScale = new Vector3 (2,2,2);
        // Enable Map Canvas and Map Camera
        mapAndArmorCanvas.enabled = true;
        mapCamera.enabled = true;

        // Disable Game Canvas
        gameCanvas.enabled = false;
        castleFinish.gameObject.SetActive(true);
        mapBackButton.gameObject.SetActive(false);

        gameManager.isFighting = false;
        playerAmbushDetection.ambushTriggered = false; //Allows to be double ambushed once the first ambush ends when caught in between 2 enemiesgit
        playerGridMovement.HideActionButton();
        gameManager.SetActiveEnemy(null);  // Clear active enemy
        gameManager.enemyHPText.gameObject.SetActive(false);
        gameManager.PlayGameWinSoundEffect();
    }
}
