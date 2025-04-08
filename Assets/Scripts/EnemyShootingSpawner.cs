using System.Collections;
using UnityEngine;

public class EnemyShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject secondatyProjectilePrefab;
    [SerializeField] private Transform spawnPoint;
    private float projectileOffset = 0;
    private GameManager gameManager;
    private Player player;
    private PlayerGridMovement playerGridMovement;
    private Enemy enemy;
    private bool isEnemyAttacking = false; // Flag to prevent multiple calls

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        enemy = GetComponent<Enemy>(); //Fetch the enemy component attachedto the same game object
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isEnemysTurn && gameManager.activeEnemy == this.GetComponent<Enemy>() &&!isEnemyAttacking)
        {
            StartCoroutine(EnemyAttack());
        }
    }

    IEnumerator EnemyAttack() {
        isEnemyAttacking = true; // Set the flag to prevent multiple calls
        yield return new WaitForSeconds(1f);
        ShootAtPlayer(player.transform);
        yield return new WaitForSeconds(1f);
        isEnemyAttacking = false; // Reset the flag after the attack
    }

    public void ShootAtPlayer(Transform playerTransform)
    {
        // Spawn and initialize the projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position + spawnPoint.forward * projectileOffset,
            Quaternion.identity
        );

        // Disable the "Billboard" and "ItemPositioning" scripts on the projectile
        Billboard billboard = projectile.GetComponent<Billboard>();
        if (billboard != null) Destroy(billboard);

        ItemPositioning itemPositioning = projectile.GetComponent<ItemPositioning>();
        if (itemPositioning != null) Destroy(itemPositioning);

        // Ignore collision between the projectile and the enemy
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider enemyCollider = GetComponent<Collider>();
        if (projectileCollider != null && enemyCollider != null)
        {
            Physics.IgnoreCollision(projectileCollider, enemyCollider);
        }

        // Initialize the projectile to target the player
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Initialize(spawnPoint.position, playerTransform.position);
        }


        StartCoroutine(DelayPlayerturn()); // Delay to give the player time to react
    }

    IEnumerator DelayPlayerturn() {
        yield return new WaitForSeconds(0.7f);
        gameManager.isEnemysTurn = false;
        gameManager.isPlayersTurn = true;
        playerGridMovement.ShowActionButton();
    }
}
