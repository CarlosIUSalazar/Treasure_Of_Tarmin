using UnityEngine;

public class CorridorDoor : MonoBehaviour
{

    FloorManager floorManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            Debug.Log("Player crossed Corridor Door");
            floorManager.MovePlayerToNewMaze(gameObject.tag);
        }
    }
}
