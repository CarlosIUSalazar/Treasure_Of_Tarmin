using UnityEngine;

public class PlayerGridMovement : MonoBehaviour
{
    public float gridSize = 1.0f; //Size of each grid step
    public float movementSpeed = 5.0f;
    public bool isMoving = false;
    
    private Vector3 targetPosition;

    void Start() {
        targetPosition = transform.position;
    }

    void Update() {
        if (!isMoving) {
            HandleInput();
        }
        
        MoveToTarget();
    }

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.W)) {
            targetPosition = transform.position + transform.forward * gridSize;
            isMoving = true;
        }

        else if (Input.GetKeyDown(KeyCode.S)) {
            targetPosition = transform.position - transform.forward * gridSize;
        }

        else if (Input.GetKeyDown(KeyCode.A)) {
            transform.Rotate(0, -90, 0);
        }

        else if (Input.GetKeyDown(KeyCode.D)) {
            transform.Rotate(0, 90, 0);
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

}


