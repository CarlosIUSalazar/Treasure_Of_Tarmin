using UnityEngine;

public class PlayerShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint; // Assign this to the position where the arrow should appear
    [SerializeField] private float projectileOffset = 1.5f; // Offset in front of the player
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;
    Player player;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
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
            if (player.arrows > 0) { // Only spawn arrows if there are any.  If not, player loses his turn.
                GameObject projectile = Instantiate(
                projectilePrefab, 
                spawnPoint.position + spawnPoint.forward * projectileOffset, 
                Quaternion.identity
                );

                player.ModifyArrows(-1);

                Projectile proj = projectile.GetComponent<Projectile>();
                if (proj != null) {
                    proj.Initialize(spawnPoint.position, enemy.position + new Vector3(0, 2f, 0)); //Pass the enemy position a bit shifted upwards as it was shooting towards the balls
                }
                Debug.Log("Arrow spawned!");
            }
            playerGridMovement.HideActionButton();
            gameManager.isEnemysTurn = true;
        }
    }
}
