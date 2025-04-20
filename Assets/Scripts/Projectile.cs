using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ItemMapping itemMapping;
    private float projectileSpeed = 12f; // Speed of the projectile
    //private float damage;
    //private int amount = -10;
    private Vector3 direction;
    ItemMapping currentPlayerWeapon; 
    ///PlayerShootingSpawner playerShootingSpawner;
    InventoryManager inventoryManager;

    void Start()
    {
        //playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
        currentPlayerWeapon = inventoryManager.FigureOutCurrentRightHandItemMapping();
    }
    // Called after instantiation to initialize the projectile
    public void Initialize(Vector3 shooterPosition, Vector3 targetPosition)
    {
        // Calculate the direction toward the target
        direction = (targetPosition - shooterPosition).normalized;

        // Set the visual rotation of the projectile with a 220-degree offset in Y for enemy projectiles
        Quaternion visualRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 235, 0); ;
        transform.rotation = visualRotation;
    }

    void Update()
    {
        // Move the projectile in the calculated direction
        transform.position += direction * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Debug.Log($"HIT {other.tag}");

            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    //The MULTIUSE WEAPON DAMAGE IS NOW CALCULATED ON ENEMY.CS
                    enemy.TakeDamage(currentPlayerWeapon);
                }
            }
            else if (other.CompareTag("Player"))
            {
                //Damage to player is calculated based on the stats of the assigned itemMapping to this Script on the editor to each weapon prefab
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.playerTakeDamageCalculation(itemMapping);
                }
            }
            Destroy(gameObject);
        }
    }
}
