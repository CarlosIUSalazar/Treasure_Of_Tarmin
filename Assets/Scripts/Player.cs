using UnityEngine;

public class Player : MonoBehaviour
{
    private int maxHP = 100;
    private int currentHP;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Player HP: {currentHP}");

        if (currentHP <= 0)
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
