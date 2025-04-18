using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ItemMapping itemMapping;
    private float projectileSpeed = 12f; // Speed of the projectile
    private float damage;
    private int amount = -10;
    private Vector3 direction;
    ItemMapping currentItemMapping; 
    PlayerShootingSpawner playerShootingSpawner;


    void Start()
    {
        playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();
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
        //Check which attack is stronger War or Spiritual, use higher, apply a bonus between 5 and 25% and convert back to int
        float damageWar = itemMapping.warAttackPower;
        float damageSpiritual = itemMapping.spiritualAttackPower;
        damage = (damageWar > damageSpiritual) ? damageWar : damageSpiritual;
        float bonusDamage = Random.Range(damage * 0.05f, damage * 0.25f);
        damage = damage + bonusDamage;
        int attackDamage = Mathf.RoundToInt(damage);
        Debug.Log("Damage is " + attackDamage);

        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Debug.Log($"HIT {other.tag}");

            if (other.CompareTag("Enemy"))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    //The MULTIUSE WEAPON DAMAGE IS NOW CALCULATED ON PLAYER SHOOTING SPAWNER, OTHER WEAPONS USE THE CALCULATION ABOVE WITHIN THIS SCRIPT
                    int usingMultiUseWeaponDamage = playerShootingSpawner.MultiUseWeaponFired();
                    Debug.Log("usingMultiUseWeaponDamage is " + usingMultiUseWeaponDamage);
                    if (usingMultiUseWeaponDamage == 0) {
                        Debug.Log("Using SingleUse Weapon");
                        enemy.TakeDamage(attackDamage);
                        playerShootingSpawner.ConsumeItem(); //MOVED THIS HERE TO AVOID NULL REFERENCE AFTER THE ITEM IS NO LONGER IN HAND
                    } else {
                        Debug.Log("Using MultiUse Weapon");
                        enemy.TakeDamage(usingMultiUseWeaponDamage);
                    }
                }
                Destroy(gameObject);
            }
            else if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.ModifyPhysicalStrength(-attackDamage);
                }
                Destroy(gameObject);
            }
        }
    }
}
