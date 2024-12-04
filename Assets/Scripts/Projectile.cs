using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction; // Direction in which the arrow moves
    private float projectileSpeed = 12f;

    // Start is called once before the first execution of Update
    void Start()
    {
        // Get the direction the arrow is facing when instantiated
        direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        // Move the arrow in the direction it's facing
        transform.position += direction * projectileSpeed * Time.deltaTime;
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
