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
    PlayerShootingSpawner playerShootingSpawner;
    GameManager gameManager;
    Enemy enemy;
    public float gridSize = 10.0f; //Size of each grid step
    public float movementSpeed = 5.0f;
    public bool isMoving = false;
    public bool isRotating = false;
    public Vector3 gridStart = new Vector3(-5, 1, -5); // Define your custom grid start position

    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start() {
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
        //Debug.DrawRay(transform.position, transform.forward * 10.0f, Color.red);

        if (gameManager.isFighting)
        {
            HideDirectionalButtons();
        } else {
            ShowDirectionalButtons();
        }
    }

    private void HideDirectionalButtons()
    {
        forwardButton.gameObject.SetActive(false);
        rotateLeftButton.gameObject.SetActive(false);
        rotateRightButton.gameObject.SetActive(false);
    }

    private void ShowDirectionalButtons()
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

        // else if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     OpenDoorButton();
        // }
    }

    public void MoveForward()
    {
        if (!isMoving && CanMoveForward())
        {
            targetPosition = GetSnappedPosition(transform.position + transform.forward * gridSize);
            isMoving = true;
        }
    }

    public void MoveBackwards()
    {
        targetPosition = transform.position - transform.forward * gridSize;
    }

    public void TurnLeft()
    {
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        isRotating = true;
    }

    public void TurnRight()
    {
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        isRotating = true;
    }

    // public void OpenDoorButton()
    // {
    //     RaycastHit hit;
    //     float rayDistance = gridSize;
    //     if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance))
    //     {
    //         if (hit.collider.CompareTag("Door"))
    //         {
    //             DoorController door = hit.collider.GetComponent<DoorController>();
    //             if (door != null)
    //             {
    //                 door.OpenDoor();
    //             }
    //         }
    //     }
    // }

    void MoveToTarget()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            // Stop movement and snap precisely when close to target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = GetSnappedPosition(targetPosition); // Ensure exact grid alignment
                isMoving = false;
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

    void RotateToTarget() {
        if (isRotating) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1d) {
                transform.rotation = targetRotation; // Snap to target rotation
                isRotating = false;
            }
        }
    }

    bool CanMoveForward() {
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
    float rayDistance = gridSize;

    // Slightly offset the ray downwards to hit items on the floor
    Vector3 rayOrigin = transform.position + new Vector3(0, -0.5f, 0); // Adjust -0.5f based on player's height
    Vector3 rayDirection = transform.forward;

    Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red); // Debug the ray in the scene view

    if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance))
    {
        if (hit.collider.CompareTag("Door"))
        {
            actionButtonText.text = "Open";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => OpenDoor(hit));
        }
        else if (hit.collider.CompareTag("Item"))
        {
            actionButtonText.text = "Pick Up";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => PickUpItem(hit));
        }
        else if (hit.collider.CompareTag("Enemy")){
            InitiateFight(hit);
            //enemy = hit.collider.GetComponent<Enemy>();
            //gameManager.isFighting = true;
            //actionButtonText.text = "Attack";
            //actionButton.onClick.RemoveAllListeners();
           // actionButton.onClick.AddListener(() => InitiateFight(hit));
        }
        else
        {
            ResetActionButton();
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

    private void ResetActionButton() {
        actionButtonText.text = "";
        actionButton.onClick.RemoveAllListeners();
    }

    private void PickUpItem(RaycastHit hit) {
        GameObject item = hit.collider.gameObject;
        Debug.Log("item is " + item.gameObject.name);
        item.SetActive(false);
        //Add pickup logic
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
}


