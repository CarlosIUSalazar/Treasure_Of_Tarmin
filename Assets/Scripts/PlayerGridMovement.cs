using UnityEngine;
using System.Collections;

public class PlayerGridMovement : MonoBehaviour
{
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
        if (!isMoving) {
            HandleInput();
        }
        
        MoveToTarget();
        RotateToTarget();
    }

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.W)) {
            if (CanMoveForward()) {
                targetPosition = transform.position + transform.forward * gridSize;
                isMoving = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            targetPosition = transform.position - transform.forward * gridSize;
        }

        else if (Input.GetKeyDown(KeyCode.A)) {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
            isRotating = true;
        }

        else if (Input.GetKeyDown(KeyCode.D)) {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
            isRotating = true;
        }

        else if (Input.GetKeyDown(KeyCode.Space)) {
            RaycastHit hit;
            float rayDistance = gridSize;

            if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) {
                if (hit.collider.CompareTag("Door")) {
                    DoorController door = hit.collider.GetComponent<DoorController>();
                    if (door != null) {
                        door.OpenDoor();
                    }
                }
            }

        }
    }

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
            return false; // Obstacle found, cannot move forward
        }
        return true;
    }

}


