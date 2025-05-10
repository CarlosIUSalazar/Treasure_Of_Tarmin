using Unity.Mathematics;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public EnemyMapping enemyMapping;  // assigned via inspector
    [SerializeField] private GameObject treasureOfTarminPrefab;
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;
    Player player;
    PlayerAmbushDetection playerAmbushDetection;
    PlayerShootingSpawner playerShootingSpawner;
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
        playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();
        GetFloorType();
        gameManager.UpdateEnemyHP(currentEnemyHP);
        SetEnemyHP();
    }


    private void GetFloorType() {
        //Tan, Blue, or Green
        currentFloorColor = gameManager.currentMazeBlock.colorType.ToString();
    }


    private void SetEnemyHP() {
        // 1) Grab the current floor
        int floor = gameManager.currentFloor;

        // 2) Pick the base HP by color
        float baseHP;
        if (currentFloorColor == "Green") {
            baseHP = enemyMapping.baseWarHP;
        }
        else if (currentFloorColor == "Blue") {
            baseHP = enemyMapping.baseSpiritualHP;
        }
        else { // Tan
            baseHP = Mathf.Max(enemyMapping.baseWarHP, enemyMapping.baseSpiritualHP);
        }
        // 3) Add a flat bump per floor
        const float HP_PER_FLOOR = 0.4f;
        float bumpedHP = baseHP + floor * HP_PER_FLOOR;

        // 4) Apply your random 5–12% bonus
        float bonus = UnityEngine.Random.Range(bumpedHP * 0.05f, bumpedHP * 0.12f);
        currentEnemyHP = Mathf.RoundToInt(bumpedHP + bonus);

        // 5) Debug log
        Debug.Log($"Set Enemy {gameObject.name} HP to {currentEnemyHP} at floor {floor}");
    }


    public void TakeDamage(ItemMapping currentPlayerWeapon)
    {
    // Tweak K up/down (15 → 35) to compress or stretch fight lengths.

    // Experiment with cutting your enemy defensePerFloor even further.

    // Remove or shrink the random bonus (e.g. 5–10% instead of 5–25%) if you want rock-solid predictability.

        gameManager.PlayPunkSoundEffect();

        // 1) Base weapon attack + small random
        float baseDamage  = Mathf.Max(currentPlayerWeapon.warAttackPower,
                                    currentPlayerWeapon.spiritualAttackPower);
        float bonusDamage = UnityEngine.Random.Range(baseDamage * 0.05f,
                                                    baseDamage * 0.15f);
        float rawAttack   = baseDamage + bonusDamage;

        // 2) Type advantage bonus (unchanged)
        float typeBonus = 0f;
        if (!enemyMapping.isHorrible)
        {
            if (currentPlayerWeapon.isWarWeapon     && enemyMapping.isSpiritual)   typeBonus = 0.05f;
            else if (currentPlayerWeapon.isSpiritualWeapon && enemyMapping.isWar) typeBonus = 0.05f;
        }

        // 3) Calculate “soft” total defense
        //    — cut defensePerFloor to 0.1 in your mappings
        //    — halve the color-to-defense multiplier
        float floorDef     = gameManager.currentFloor * enemyMapping.defensePerFloor;
        float colorDef     = enemyMapping.colorMultiplier * 5f;
        float totalDefense = enemyMapping.baseDefense
                            + floorDef
                            + enemyMapping.shieldBonus
                            + colorDef;

        // 4) Diminishing-returns mitigation
        // K controls how “hard” defense hits you; lower K => less mitigation
        // The mitigation = def/(def+K) curve tapers off as def grows—you never hit the “linear 100% at def=50” cliff.
        // With K=25, an enemy at totalDefense≈40 only blocks ≈62% of your damage, so your 64-point weapon punches for ~24 points.
        // A 5 → 8 HP “trash” mob drop in 2–3 hits, a 60–80 HP elite in 3–5 hits, and the occasional mini-boss in 6–8 hits, which feels snappy even all the way to floor 255.
        
        // Tweak K up/down (15 → 35) to compress or stretch fight lengths.
        // Experiment with cutting your enemy defensePerFloor even further.
        // Remove or shrink the random bonus (e.g. 5–10% instead of 5–25%) if you want rock-solid predictability.
        
        const float K = 25f; // tweak this to taste from 15 to 35
        float mitigation = totalDefense / (totalDefense + K);
        mitigation = Mathf.Clamp(mitigation, 0f, 0.9f); // never more than 90% reduction

        // 5) Final damage
        float finalDamageF = rawAttack * (1f + typeBonus) * (1f - mitigation);
        int   finalDamage  = Mathf.Max(1, Mathf.RoundToInt(finalDamageF));

        // 6) Apply & log
        currentEnemyHP -= finalDamage;
        Debug.Log($"Hit for {finalDamage} (raw {rawAttack:F1}, def {totalDefense:F1}, mitig {mitigation:P0})");

        // 7) Consume single-use or not
        if (!currentPlayerWeapon.isMultiUseWeapon)
            inventoryManager.EmptyRightHand();

        playerShootingSpawner.CheckWeaponBreakingChance(currentPlayerWeapon);

        // 8) Refresh UI / check death
        gameManager.UpdateEnemyHP(currentEnemyHP);
        if (currentEnemyHP <= 0) Die();
    }

    // public void TakeDamage(ItemMapping currentPlayerWeapon)
    // {
    //     // 1. Determine base attack value
    //     float damageWar = currentPlayerWeapon.warAttackPower;
    //     float damageSpiritual = currentPlayerWeapon.spiritualAttackPower;
    //     bool weaponIsWar = currentPlayerWeapon.isWarWeapon;
    //     bool weaponIsSpiritual = currentPlayerWeapon.isSpiritualWeapon;
        
    //     //Check which attack is stronger War or Spiritual, use higher, apply a bonus between 5 and 25% and convert back to int
    //     float baseDamage = Mathf.Max(damageWar, damageSpiritual);

    //     // 2. Optional: Apply bonus variation (for randomness)
    //     float bonusDamage = UnityEngine.Random.Range(baseDamage * 0.05f, baseDamage * 0.25f);
    //     float rawAttack = baseDamage + bonusDamage;

    //     // 3. Type bonus
    //     float typeBonus = 0f;
    //     if (!enemyMapping.isHorrible) // Horrible monsters take no bonus damage, others take 5% if an opposite type weapon is used
    //     {
    //         if (weaponIsWar && enemyMapping.isSpiritual)
    //             typeBonus = 0.05f; // War weapon vs Spiritual enemy

    //         else if (weaponIsSpiritual && enemyMapping.isWar)
    //             typeBonus = 0.05f; // Spiritual weapon vs War enemy
    //     }

    //     // 4. Calculate total defense
    //     float floorDefense = gameManager.currentFloor * enemyMapping.defensePerFloor;
    //     float colorDefense = enemyMapping.colorMultiplier * 10f;
    //     float totalDefense = enemyMapping.baseDefense + floorDefense + enemyMapping.shieldBonus + colorDefense;

    //     // 5. Final damage formula
    //     float finalDamage = rawAttack * (1 + typeBonus) * (1 - (totalDefense / 50f));
    //     finalDamage = Mathf.Max(finalDamage, 1); // prevent zero or negative damage

    //     // 6. Apply and log
    //     currentEnemyHP -= Mathf.RoundToInt(finalDamage);

    //     Debug.Log($"Final Damage Dealt: {Mathf.RoundToInt(finalDamage)} (Raw: {rawAttack}, Defense: {totalDefense}, Type Bonus: {typeBonus})");

    //     if (currentPlayerWeapon.isMultiUseWeapon)
    //     {
    //         Debug.Log("Using MULTIUSE WEAPON");
    //     }
    //     else
    //     {
    //         Debug.Log("Using SINGLE-USE WEAPON, consuming...");
    //         inventoryManager.EmptyRightHand();
    //     }

    //     ////
    //     //Check For Weapon Breaking 
    //     playerShootingSpawner.CheckWeaponBreakingChance(currentPlayerWeapon);

    //     // Update UI and check for death
    //     gameManager.UpdateEnemyHP(currentEnemyHP);
    //     if (currentEnemyHP <= 0)
    //     {
    //         Die();
    //     }
    // }


    public void Die() {
        Debug.Log("Enemy Defeated: " + gameObject.name);
        if (gameObject.name == "Minotaur.vox(Clone)") {
            Instantiate(treasureOfTarminPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
        gameManager.isFighting = false;
        //player.CheckIfCanRest();
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
