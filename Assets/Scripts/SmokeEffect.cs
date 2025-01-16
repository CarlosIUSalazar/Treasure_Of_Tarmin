using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    [SerializeField] private GameObject smokeFrame1; // Assign Smoke1 in the Inspector
    [SerializeField] private GameObject smokeFrame2; // Assign Smoke2 in the Inspector
    private float switchInterval = 0.1f; // Time between frame switches
    private float duration = 2f; // Total duration of the smoke effect
    private float upwardSpeed = 1f; // Speed at which the smoke moves upward

    private float elapsedTime = 0f;
    private float frameTimer = 0f;
    private bool isFrame1Active = true;

    void Start()
    {
        if (!smokeFrame1 || !smokeFrame2)
        {
            Debug.LogError("Smoke frames not assigned in SmokeEffect script.");
        }
    }

    void Update()
    {
        // Move the prefab upward
        transform.position += Vector3.up * upwardSpeed * Time.deltaTime;

        // Switch between Smoke1 and Smoke2
        frameTimer += Time.deltaTime;
        if (frameTimer >= switchInterval)
        {
            frameTimer = 0f;
            isFrame1Active = !isFrame1Active;

            smokeFrame1.SetActive(isFrame1Active);
            smokeFrame2.SetActive(!isFrame1Active);
        }

        // Track elapsed time
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration)
        {
            Destroy(gameObject); // Destroy the prefab after the effect finishes
        }
    }
}
