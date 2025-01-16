using UnityEngine;

public class Enemy : MonoBehaviour
{
    //[SerializeField] private string enemyName = "WhiteSkeleton";
    [SerializeField] private int enemyBaseHP = 50;
    public GameObject smokePrefab; // Assign SmokePrefab in the Inspector
    GameManager gameManager;
    private int currentHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        currentHP = Random.Range(0,15) + enemyBaseHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) {
        currentHP -= damage;
        Debug.Log("Enemy current HP is " + currentHP);
        if (currentHP <= 0) {
            Die();
        }
    }

    public void Die() {
        Debug.Log("Enemy Defeated");
        Destroy(gameObject);
        gameManager.isFighting = false;
        // Trigger exploration mode after combat ends
        gameManager.isExploring = true;
        // Refresh UI immediately after combat
        gameManager.SetActiveEnemy(null);  // Clear active enemy
        Instantiate(smokePrefab, transform.position, Quaternion.identity);

    }
}
