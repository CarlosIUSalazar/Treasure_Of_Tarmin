using UnityEngine;

public class PlayerShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint; // Assign this to the position where the arrow should appear
    [SerializeField] private float projectileOffset = 1.5f; // Offset in front of the player
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;
    
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootAtEnemy(Transform enemy)
    {
        if (gameManager.isPlayersTurn) {
            // Instantiate the arrow at the spawn point, matching the player's rotation
            gameManager.isPlayersTurn = false;

            //Spawn and initialize the projectile
            GameObject projectile = Instantiate(
                projectilePrefab, 
                spawnPoint.position + spawnPoint.forward * projectileOffset, 
                Quaternion.identity
            );

            Projectile proj = projectile.GetComponent<Projectile>();
            if (proj != null) {
                proj.Initialize(spawnPoint.position, enemy.position); //Pass the enemy position
            }
            Debug.Log("Arrow spawned!");
            playerGridMovement.HideActionButton();
            gameManager.isEnemysTurn = true;
        }
    }
}
