using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private string enemyName = "WhiteSkeleton";
    [SerializeField] private int enemyMaxHP = 50;
    private int currentHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = enemyMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) {
        currentHP -= damage;

        if (currentHP <= 0) {
            Die();
        }
    }

    public void Die() {
        Debug.Log("Enemy Defeated");
        Destroy(gameObject);
    }
}
