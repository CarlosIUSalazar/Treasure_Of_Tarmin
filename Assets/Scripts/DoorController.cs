// using System.Collections;
// using UnityEngine;

// public class DoorController : MonoBehaviour
// {
//     private float liftHeight = 6.0f;
//     private float liftSpeed = 5.0f;
//     Vector3 initialPosition;
//     Vector3 targetPosition;
//     bool isOpen = false;


//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         initialPosition = transform.position;
//         targetPosition = initialPosition + new Vector3(0, liftHeight, 0);
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }

//     public void OpenDoor() {
//         if (!isOpen)
//         {
//             StartCoroutine (LiftDoor());
//             isOpen = true;
//         }
//     }

//     private IEnumerator LiftDoor()
//     {
//         while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
//         {
//             transform.position = Vector3.MoveTowards(transform.position, targetPosition, liftSpeed * Time.deltaTime);
//             yield return null;
//         }
//     }
// }

using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    private float liftHeight = 6.0f;
    private float liftSpeed = 5.0f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isOpen = false;

    // Maximum distance at which the door can be interacted with.
    private float maxInteractDistance = 10.0f;
    // Minimum dot product value to ensure the door is in front of the player.
    private float minForwardDot = 0.8f;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, liftHeight, 0);
    }

    void OnMouseDown()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        // Check distance from player to door.
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > maxInteractDistance)
        {
            Debug.Log("Door is too far away to open.");
            return;
        }

        // Check if the door is roughly in front of the player.
        Vector3 toDoor = (transform.position - player.transform.position).normalized;
        float dot = Vector3.Dot(player.transform.forward, toDoor);
        if (dot < minForwardDot)
        {
            Debug.Log("Door is not in front of the player.");
            return;
        }
        // If all checks pass, open the door.
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            StartCoroutine(LiftDoor());
            isOpen = true;
        }
    }

    private IEnumerator LiftDoor()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, liftSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
