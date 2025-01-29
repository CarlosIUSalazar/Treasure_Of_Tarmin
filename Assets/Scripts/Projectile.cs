using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float projectileSpeed = 12f; // Speed of the projectile
    private int damage = 10;
    private int amount = -10;
    private Vector3 direction;

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
                    enemy.TakeDamage(damage);
                }
            }
            else if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.ModifyPhysicalStrength(amount);
                }
            }

            Destroy(gameObject);
        }
    }
}
