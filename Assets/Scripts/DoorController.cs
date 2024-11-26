using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private float liftHeight = 6.0f;
    private float liftSpeed = 5.0f;
    Vector3 initialPosition;
    Vector3 targetPosition;
    bool isOpen = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, liftHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor() {
        if (!isOpen)
        {
            StartCoroutine (LiftDoor());
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
