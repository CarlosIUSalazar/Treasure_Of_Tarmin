using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerGridMovement : MonoBehaviour
{
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    public float gridSize = 10.0f; //Size of each grid step
    public float movementSpeed = 5.0f;
    public bool isMoving = false;
    public bool isRotating = false;
    
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start() {
        targetPosition = transform.position;
    }

    void Update() {
        if (!isMoving && !isRotating) {
            HandleInput();
            CheckForInteractables();

        }
        MoveToTarget();
        RotateToTarget();
        //Debug.DrawRay(transform.position, transform.forward * 10.0f, Color.red);

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
        if (CanMoveForward())
        {
            targetPosition = transform.position + transform.forward * gridSize;
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

    void MoveToTarget() {
        if (isMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
        // Stop movement when close to target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            transform.position = targetPosition;
            isMoving = false;
        }
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
            actionButtonText.text = "Attack";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => InitiateFight(hit));
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
        item.SetActive(false);
        //Add pickup logic
    }

    private void InitiateFight(RaycastHit hit) {
        //Initiate Fight Mode
    }

}


