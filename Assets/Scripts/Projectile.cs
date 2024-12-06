using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float projectileSpeed = 12f; // Speed of the arrow

    // Update is called once per frame
    void Update()
    {
        // Move the arrow forward based on its local forward direction
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("HIT");
            Destroy(gameObject);
        }
    }
}