using UnityEngine;

public class Player : MonoBehaviour
{
    private int maxPhysicalStrength = 100;
    private int currentPhysicalStrength;

    GameManager gameManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.SetPhysicalStrength(100);
        currentPhysicalStrength = maxPhysicalStrength;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentPhysicalStrength -= damage;
        Debug.Log($"Player PhysicalStrength: {currentPhysicalStrength}");

        if (currentPhysicalStrength <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Defeated!");
        // Add logic for player death, like restarting the level or showing a game-over screen
        gameObject.SetActive(false);
    }
}
