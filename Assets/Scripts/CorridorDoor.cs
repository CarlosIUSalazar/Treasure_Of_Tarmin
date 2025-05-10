using UnityEngine;

public class CorridorDoor : MonoBehaviour
{

    FloorManager floorManager;
    Player player;
    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")){
            Debug.Log("Player crossed Corridor Door");
            gameManager.PlaySwooshSoundEffect();
            player.CorridorDoorCrossingHP(gameObject.name);
            floorManager.MovePlayerToNewMazeViaCorridorDoor(gameObject.tag);
        }
    }
}
