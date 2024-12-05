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

    public void ShootArrow() {
        //Instantiate(arrow, transform.position + new Vector3(2,0,0.5f), Quaternion.Euler(0,180,0));
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position + transform.forward * arrowOffset, transform.rotation);

    }

}
