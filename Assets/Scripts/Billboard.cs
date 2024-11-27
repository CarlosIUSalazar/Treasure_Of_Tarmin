using UnityEngine;

public class Billboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make the object look at the camera
        transform.LookAt(Camera.main.transform);

        // Lock the object's rotation to the Y-axis only (prevent tilting)
        transform.rotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
    }
}
