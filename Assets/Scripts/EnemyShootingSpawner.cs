using System.Collections;
using UnityEngine;

public class EnemyShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float projectileOffset = -1.5f;
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
        isEnemyAttacking = false; // Reset the flag after the attack
    }

    public void ShootAtPlayer(Transform player) {
        //Spawn and initialize the projectile
        GameObject projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position + spawnPoint.forward * projectileOffset,
            Quaternion.identity
        );

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null) {
            proj.Initialize(spawnPoint.position, player.position); //Pass the target (player) position
        }
        gameManager.isEnemysTurn = false;
        playerGridMovement.ShowActionButton();
        gameManager.isPlayersTurn = true;
    }
}
