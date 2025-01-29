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
        if (gameManager.isPlayersTurn)
        {
            gameManager.isPlayersTurn = false;

            if (player.arrows > 0) // Only shoot if the player has arrows
            {
                // Instantiate the dart (projectile) with an additional rotation of 220 degrees in Y
                GameObject projectile = Instantiate(
                    projectilePrefab,
                    spawnPoint.position + spawnPoint.forward * projectileOffset,
                    Quaternion.identity // Use identity rotation; the offset will be handled in Initialize in Projectile.cs
                );

                // Disable the "Billboard" and "ItemPositioning" scripts on the projectile
                Billboard billboard = projectile.GetComponent<Billboard>();
                if (billboard != null) Destroy(billboard);

                ItemPositioning itemPositioning = projectile.GetComponent<ItemPositioning>();
                if (itemPositioning != null) Destroy(itemPositioning);

                // Deduct one arrow from the player's inventory
                player.ModifyArrows(-1);

                // Initialize the projectile
                Projectile proj = projectile.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Initialize(spawnPoint.position, enemy.position + new Vector3(0, 2f, 0)); //2f vertical to shoot higher at the enemy
                }
                Debug.Log("Dart shot!");
            }

            playerGridMovement.HideActionButton();
            gameManager.isEnemysTurn = true; // Switch to the enemy's turn
        }
    }



}
