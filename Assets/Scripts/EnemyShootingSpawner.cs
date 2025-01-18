using System.Collections;
using UnityEngine;

public class EnemyShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
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
        yield return new WaitForSeconds(1.5f);
        ShootAtPlayer(player.transform);
        yield return new WaitForSeconds(0.5f);
        isEnemyAttacking = false; // Reset the flag after the attack
    }

    public void ShootAtPlayer(Transform player) {
        //Spawn and initialize the projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position + spawnPoint.forward * projectileOffset,
            Quaternion.identity
        );

        // Ignore collision between the projectile and the enemy
        Collider projectileCollider = projectile.GetComponent<Collider>();
        Collider enemyCollider = GetComponent<Collider>();
        if (projectileCollider != null && enemyCollider != null) {
            Physics.IgnoreCollision(projectileCollider, enemyCollider);
        }

        //Initialize projectile
        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null) {
            proj.Initialize(spawnPoint.position, player.position); //Pass the target (player) position
        }
        gameManager.isEnemysTurn = false;
        StartCoroutine(DelayPlayerturn()); //I delayed it in order to avoid escaping too soon and triggering one more enemy attack, it could be better
    }

    IEnumerator DelayPlayerturn() {
        yield return new WaitForSeconds(1f);
        playerGridMovement.ShowActionButton();
        gameManager.isPlayersTurn = true;
    }
}
