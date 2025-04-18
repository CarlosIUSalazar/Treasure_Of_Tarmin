using Unity.Mathematics;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyMapping enemyMapping;  // assigned via inspector
    [SerializeField] private GameObject treasureOfTarminPrefab;
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;
    Player player;
    PlayerAmbushDetection playerAmbushDetection;
    FloorManager floorManager;
    MazeBlock mazeBlock;
    InventoryManager inventoryManager;
    private string currentFloorColor;
    private int enemyBaseHP;
    public GameObject smokePrefab; // Assign SmokePrefab in the Inspector
    public int currentEnemyHP;
    private float maxInteractionDistance = 5f;
    public float gridSize = 10.0f; //Size of each grid step
    // Detection / Ambush
    [Header("Ambush Settings")]
    //public float detectionDistance = 10f;     // How far the enemy can see forward
    public float detectionTimeRequired = 2f; // How many seconds the player must stay in sight
    private float timePlayerInSight = 0f;
    private bool hasAmbushed = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerAmbushDetection = GameObject.Find("Player").GetComponent<PlayerAmbushDetection>();
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();        
        GetFloorType();
        gameManager.UpdateEnemyHP(currentEnemyHP);
        SetEnemyHP();
    }


    private void GetFloorType() {
        //Tan, Blue, or Green
        currentFloorColor = gameManager.currentMazeBlock.colorType.ToString();
    }


    private void SetEnemyHP() {
        //FLOOR BASED HP
        if (currentFloorColor == "Green") { //WAR FLOOR
            float baseHP = enemyMapping.baseWarHP;
            float bonus = UnityEngine.Random.Range(baseHP * 0.05f, baseHP * 0.25f);
            currentEnemyHP = Mathf.RoundToInt(baseHP + bonus);
            Debug.Log("Set Enemy " + gameObject.name + " HP of " + currentEnemyHP);
        } else if (currentFloorColor == "Blue") { //SPIRITUAL FLOOR
            float baseHP = enemyMapping.baseSpiritualHP;
            float bonus = UnityEngine.Random.Range(baseHP * 0.05f, baseHP * 0.25f);
            currentEnemyHP = Mathf.RoundToInt(baseHP + bonus);
            Debug.Log("Set Enemy " + gameObject.name + " HP of " + currentEnemyHP);
        }  else if (currentFloorColor == "Tan") { //MIXED FLOOR (USES HIGHER HP)
            float baseSpiritualHP = enemyMapping.baseSpiritualHP;
            float baseWarHP = enemyMapping.baseWarHP;
            float baseHP = (baseSpiritualHP > baseWarHP) ? baseSpiritualHP : baseWarHP;
            float bonus = UnityEngine.Random.Range(baseHP * 0.05f, baseHP * 0.25f);
            currentEnemyHP = Mathf.RoundToInt(baseHP + bonus);
            Debug.Log("Set Enemy " + gameObject.name + " HP of " + currentEnemyHP);
        }
    }


    public void TakeDamage(ItemMapping currentPlayerWeapon) {
        //Check which attack is stronger War or Spiritual, use higher, apply a bonus between 5 and 25% and convert back to int
        float damageWar = currentPlayerWeapon.warAttackPower;
        float damageSpiritual = currentPlayerWeapon.spiritualAttackPower;
        float damage = (damageWar > damageSpiritual) ? damageWar : damageSpiritual;
        float bonusDamage = UnityEngine.Random.Range(damage * 0.05f, damage * 0.25f);
        damage = damage + bonusDamage;
        int attackDamage = Mathf.RoundToInt(damage);
        Debug.Log("Damage is " + attackDamage);
        if (currentPlayerWeapon.isMultiUseWeapon) {
            Debug.Log("Using MULTIUSE WEAPON dmg is" + attackDamage); 
        }else{
            Debug.Log("Using SINGLE-USE WEAPON dmg is" + attackDamage); 
            //Consume single use weapons
            inventoryManager.EmptyRightHand();
        }       
        currentEnemyHP -= attackDamage;
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
        playerAmbushDetection.ambushTriggered = false; //Allows to be double ambushed once the first ambush ends when caught in between 2 enemiesgit
        playerGridMovement.HideActionButton();
        // Trigger exploration mode after combat ends
        gameManager.isExploring = true;
        // Refresh UI immediately after combat
        gameManager.SetActiveEnemy(null);  // Clear active enemy
        Instantiate(smokePrefab, transform.position, Quaternion.identity);
        gameManager.enemyHPText.gameObject.SetActive(false);
    }
}
