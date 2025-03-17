//sing Unity.Mathematics;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //[SerializeField] private string enemyName = "WhiteSkeleton";
    [SerializeField] private int enemyBaseHP = 50;
    [SerializeField] private GameObject treasureOfTarminPrefab;
    public GameObject smokePrefab; // Assign SmokePrefab in the Inspector
    GameManager gameManager;
    public int currentEnemyHP;
    PlayerGridMovement playerGridMovement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        currentEnemyHP = Random.Range(0,15) + enemyBaseHP;
        gameManager.UpdateEnemyHP(currentEnemyHP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage) {
        currentEnemyHP -= damage;
        Debug.Log("Enemy current HP is " + currentEnemyHP);
        gameManager.UpdateEnemyHP(currentEnemyHP);
        if (currentEnemyHP <= 0) {
            Die();
        }
    }

    public void Die() {
        Debug.Log("Enemy Defeated: " + gameObject.name);
        if (gameObject.name == "Minotaur.vox(Clone)") {
            Instantiate(treasureOfTarminPrefab, transform.position, Quaternion.identity);
        }


        Destroy(gameObject);
        gameManager.isFighting = false;
        playerGridMovement.HideActionButton();
        // Trigger exploration mode after combat ends
        gameManager.isExploring = true;
        // Refresh UI immediately after combat
        gameManager.SetActiveEnemy(null);  // Clear active enemy
        Instantiate(smokePrefab, transform.position, Quaternion.identity);
        gameManager.enemyHPText.gameObject.SetActive(false);
    }
}
