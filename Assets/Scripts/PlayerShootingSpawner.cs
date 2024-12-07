using UnityEngine;

public class PlayerShootingSpawner : MonoBehaviour
{

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint; // Assign this to the position where the arrow should appear
    [SerializeField] private float arrowOffset = 1.5f; // Offset in front of the player

    GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void ShootArrow() {
    //     Instantiate(arrowPrefab, transform.position + new Vector3(2,0,0.5f), Quaternion.Euler(0,180,0));
    // }

    public void ShootArrow()
        {
            // Instantiate the arrow at the spawn point, matching the player's rotation
            GameObject arrow = Instantiate(
                arrowPrefab, 
                arrowSpawnPoint.position + arrowSpawnPoint.forward * arrowOffset, 
                arrowSpawnPoint.rotation
            );

            Debug.Log("Arrow spawned!");
        }

}
