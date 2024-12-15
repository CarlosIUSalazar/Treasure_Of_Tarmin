using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float projectileSpeed = 12f; // Speed of the arrow
    private int damage = 10;
    private Vector3 direction;

    // Called after instantiation to initialize the projectile
    public void Initialize(Vector3 shooterPosition, Vector3 targetPosition) {
        direction = (targetPosition - shooterPosition).normalized;
        transform.forward = direction; //Rotate the projectile to the target  
    }

    // Update is called once per frame
    void Update()
    {
        // Move the arrow forward based on its local forward direction
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            Debug.Log("HIT {other.tag}");

            if (other.CompareTag("Enemy")) {                
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.TakeDamage(damage);
                }
            }
            else if (other.CompareTag("Player")) {
                Player player = other.GetComponent<Player>(); //TODO: Add Player script
                if (player != null) {
                    player.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
    }
}