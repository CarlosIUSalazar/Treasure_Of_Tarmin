using System.Collections;
using UnityEngine;

public class EnemyShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    //[SerializeField] private GameObject secondaryProjectilePrefab;
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
        string floorColor = gameManager.currentMazeBlockColor;
        Enemy activeEnemy = gameManager.activeEnemy;
        EnemyMapping enemyMapping = activeEnemy.enemyMapping;
        GameObject projectileShot = projectilePrefab;
        Debug.Log("Enemy floor color is " + floorColor);
        Debug.Log("Enemy type" + enemyMapping.isHorrible);
        //FOR HORRIBLE MONSTERS PRIMARY PREFAB IS SPIRITUAL AND SECONDATRY IS WAR
        if (enemyMapping.isHorrible && floorColor == "Blue") { //SPIRITUAL
            projectileShot = enemyMapping.projectilePrefab;
        } else if (enemyMapping.isHorrible && floorColor == "Green") {
            projectileShot = enemyMapping.secondaryProjectilePrefab; //WAR
        } else if (enemyMapping.isHorrible && floorColor == "Tan") {
            int fiftyFifty = Random.Range(0,2);
            if (fiftyFifty <= 0) {  //50-50 chance of shooting war or spiritual on Tan floors. Blue floor (spiritual is the default here as per the Enemy mappings of Projectile prefab and Secondary projctile prefad)
                Debug.Log("War Attack!");
                projectileShot = enemyMapping.secondaryProjectilePrefab;
            } else {
                projectileShot = enemyMapping.projectilePrefab;
                Debug.Log("Spiritual Attack!");
            }
        } 

        //FOR MINOTAUR REGARDLESS OF THE FLOOR TYPE ATTACKS RANDOMLY
        if (enemyMapping.isMinotaur) {
            int fiftyFifty = Random.Range(0,2);
            if (fiftyFifty == 0) {  //50-50 chance of shooting war or spiritual on Tan floors. Blue floor (spiritual is the default here as per the Enemy mappings of Projectile prefab and Secondary projctile prefad)
                Debug.Log("War Attack!");
                projectileShot = enemyMapping.secondaryProjectilePrefab;
            } else {
                Debug.Log("Spiritual Attack!");
                projectileShot = enemyMapping.projectilePrefab;
            }
        }

        Debug.Log("Enemy shot " + projectileShot);
        if (projectileShot.name.Contains("Fireball"))
            gameManager.PlayFireballSoundEffect();
        if (projectileShot.name.Contains("Lightning")) 
            gameManager.PlayThunderSoundEffect();

        // Spawn and initialize the projectile
        GameObject projectile = Instantiate(
            projectileShot,
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
