using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerGridMovement : MonoBehaviour
{
    [SerializeField] private Button actionButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private Button restButton;

    PlayerShootingSpawner playerShootingSpawner;
    GameManager gameManager;
    Enemy enemy;
    Player player;
    public float gridSize = 10.0f; //Size of each grid step
    public float movementSpeed = 5.0f;
    public bool isMoving = false;
    public bool isRotating = false;
    public Vector3 gridStart = new Vector3(-5, 1, -5); // Define your custom grid start position
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();

        // Snap player to grid center at the start
        transform.position = GetSnappedPosition(transform.position);
        targetPosition = transform.position; // Align targetPosition to snapped position
    }

    void Update() {
        if (!isMoving && !isRotating) {
            HandleInput();
            CheckForInteractables();
        }
        MoveToTarget();
        RotateToTarget();
        PlayerRest();

        if (gameManager.isFighting)
        {
            HideDirectionalButtons();
        } else {
            ShowDirectionalButtons();
            CheckForInteractables();
        }
    }

    public void HideDirectionalButtons()
    {
        forwardButton.gameObject.SetActive(false);
        rotateLeftButton.gameObject.SetActive(false);
        rotateRightButton.gameObject.SetActive(false);
    }

    public void ShowDirectionalButtons()
    {
        forwardButton.gameObject.SetActive(true);
        rotateLeftButton.gameObject.SetActive(true);
        rotateRightButton.gameObject.SetActive(true);
    }

    public void HideActionButton() {
        actionButton.gameObject.SetActive(false);
    }

    public void ShowActionButton() {
        actionButton.gameObject.SetActive(true);
    }

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveForward();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveBackwards();
        }

        else if (Input.GetKeyDown(KeyCode.A))
        {
            TurnLeft();
        }

        else if (Input.GetKeyDown(KeyCode.D))
        {
            TurnRight();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenDoorButton();
        }
    }

    public void MoveForward()
    {
        if (isMoving || isRotating)  return; // Prevent movement if already moving or rotating
        if (CanMoveForward()) {
            isMoving = true;
            targetPosition = GetSnappedPosition(transform.position + transform.forward * gridSize);
        }
    }

    public void MoveBackwards()
    {
        if (isMoving || isRotating) return; // Prevent movement if already moving or rotating
        isMoving = true;
        targetPosition = GetSnappedPosition(transform.position - transform.forward * gridSize);
    }

    public void TurnLeft()
    {
        if (isMoving || isRotating) return; // Prevent rotation if already moving or rotating
        isRotating = true;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
    }

    public void TurnRight()
    {
        if (isMoving || isRotating) return; // Prevent rotation if already moving or rotating
        isRotating = true;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
    }

    private void MoveToTarget()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            // Stop movement and snap to grid when close to target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = GetSnappedPosition(targetPosition); // Ensure exact grid alignment
                isMoving = false;
            }
        }
    }

    private void RotateToTarget()
    {
        if (isRotating && !isMoving) // Ensure rotation occurs only when not moving
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);

            // Stop rotation and snap to target rotation when close
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation; // Snap to target rotation
                isRotating = false;
            }
        }
    }

    private Vector3 GetSnappedPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round((position.x - gridStart.x) / gridSize) * gridSize + gridStart.x,
            gridStart.y, // Keep the fixed Y position (1 in this case)
            Mathf.Round((position.z - gridStart.z) / gridSize) * gridSize + gridStart.z
        );
    }

    private bool CanMoveForward() {
        RaycastHit hit;
        float rayDistance = gridSize;

        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) {
            Debug.Log("Obstacle detected: " + hit.collider.name);
            if (hit.collider.CompareTag("Item")) {
                return true; //Allow to pass through Items
            } else {
            return false; // Obstacle found, cannot move forward
            }
        }
        return true;
    }

    private void CheckForInteractables()
    {
        RaycastHit hit;
        float itemDetectionDistance = gridSize * 0.5f;  // Detect items within half the grid
        float interactionDistance = gridSize;           // Detect doors/enemies one grid away

        Vector3 rayOrigin = transform.position + new Vector3(0, -0.5f, 0); // Slight downward offset
        Vector3 rayDirection = transform.forward;

        // Short raycast for items
        Debug.DrawRay(rayOrigin, rayDirection * itemDetectionDistance, Color.blue); 

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, itemDetectionDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                actionButtonText.text = "Pick Up";
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() => PickUpItem(hit));
                return; // Return to avoid triggering further checks
            }
        }

        // Long raycast for doors and enemies
        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                actionButtonText.text = "Open";
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(() => OpenDoor(hit));
            }
            else if (hit.collider.CompareTag("Enemy"))
            {
                InitiateFight(hit);
            }
        }
        else
        {
            ResetActionButton();
        }
    }

    private void OpenDoor(RaycastHit hit) {
        DoorController door = hit.collider.GetComponent<DoorController>();
        if (door != null) {
            door.OpenDoor();
        }
    }

    public void OpenDoorButton()
    {
        RaycastHit hit;
        float rayDistance = gridSize;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }
    }

    private void ResetActionButton() {
        actionButtonText.text = "";
        actionButton.onClick.RemoveAllListeners();
    }

    private void PickUpItem(RaycastHit hit) {
        GameObject item = hit.collider.gameObject;
        Debug.Log("item is " + item.gameObject.name);
        
        string itemName = hit.collider.gameObject.name;

        switch (itemName) {
            case "Flour(Clone)":
                player.ModifyFood(10);
                Debug.Log("Picked up flour");
                break;
            case "Quiver(Clone)":
                player.ModifyArrows(10);
                Debug.Log("Picked up 10 Arrows");
                break;
        }
        
        item.SetActive(false);
    }

    private void InitiateFight(RaycastHit hit) {
        enemy = hit.collider.GetComponent<Enemy>();
        if (enemy != null) {
            gameManager.isFighting = true; //initiate fight more
            gameManager.SetActiveEnemy(enemy); //Register current enemy as active
            actionButtonText.text = "Attack";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => playerShootingSpawner.ShootAtEnemy(enemy.transform));
        }
    }

    public void PlayerRest() {
        Debug.Log("Player Rested");
        restButton.onClick.RemoveAllListeners();
        restButton.onClick.AddListener(() => player.Rest());
    }
}