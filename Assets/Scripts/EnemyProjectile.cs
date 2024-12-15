using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float projectileSpeed = 12f; // Speed of the arrow
    private int damage = 10;
    // Update is called once per frame
    void Update()
    {
        // Move the arrow forward based on its local forward direction
        transform.position -= transform.up * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("HIT Player");

            // Enemy enemy = other.GetComponent<Enemy>();
            // if (enemy != null) {
            //     enemy.TakeDamage(damage);
            // }
            Destroy(gameObject);
        }
    }
}