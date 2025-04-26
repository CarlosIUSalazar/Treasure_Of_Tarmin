using UnityEngine;

public class Player : MonoBehaviour
{
    // Full total:
    // 199/99 HP
    // 119/52 Def
    private int totalMaxPhysicalStrength = 199;
    private int totalMaxSpiritualStrength = 99;
    private int totalMaxPhysicalArmor = 199;
    private int totalMaxSpiritualArmor = 52;
    
    public int currentMaxPotentialPhysicalStrength = 12; //Initial no book cap
    public int currentMaxPotentialSpiritualStrength = 6; //Initial no booko cap
    
    public int currentWarBookCurrentCapHP = 49;
    public int currentSpiritualBookCurrentCapHP = 29;

    public int physicalStrength;
    public int physicalArmor;
    public int physicalWeapon;
    public int spiritualStrength;
    
    public int spiritualArmor;
    public int spiritualWeapon;
    
    public int score;
    public int arrows;
    public int food;
    //public int floor;
    public bool canRest = false;
    
    public int timesHitLastBattle = 0;
    public bool lastHitWasWar;
    
    GameManager gameManager;
    InventoryManager inventoryManager;
    MazeGenerator mazeGenerator;
    PlayerGridMovement playerGridMovement;
    PlayerAmbushDetection playerAmbushDetection;

    //Events to notify UI changes
    public delegate void OnStatChanged();
    public event OnStatChanged OnPlayerStatsUpdated;


    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
        mazeGenerator = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        playerAmbushDetection = GameObject.Find("Player").GetComponent<PlayerAmbushDetection>();
    }


    void Start()
    {
        InitializeValues();
    }


    void Update()
    {
        CheckIfCanRest();
    }

    private void InitializeValues() {
        physicalStrength = 12;
        physicalArmor = 0;
        physicalWeapon = 0;
        spiritualStrength = 6;
        spiritualArmor = 0;
        spiritualWeapon = 0;
        score = 0;
        arrows = 10;
        food = 10;
        gameManager.currentFloor = 1;

        //Trigger UI update at start
        OnPlayerStatsUpdated?.Invoke();
    }


    public void RestoreMaxPhysicalStrengthWithSmallBluePotion() {
        physicalStrength = currentMaxPotentialPhysicalStrength;
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        OnPlayerStatsUpdated?.Invoke();
    }


    public void IncreasePhysicalScoreBy10WithLargeBluePotion() {
        if (physicalStrength + 10 >= 199) {
            totalMaxPhysicalStrength = 199;
            currentMaxPotentialPhysicalStrength = 199;
            physicalStrength = 199;
        } else {
            totalMaxSpiritualStrength += 10;
            currentMaxPotentialPhysicalStrength += 10;
            physicalStrength += 10;
        }
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        Debug.Log("totalMaxPhysicalStrength is " + totalMaxSpiritualStrength);
        Debug.Log("CurrentMaxPhysicalStrength is " + currentMaxPotentialPhysicalStrength);

        OnPlayerStatsUpdated?.Invoke();
    }


    public void IncreaseSpiritualScoreBy10WithLargePinkPotion() {
        if (spiritualStrength + 10 >= 99) {
            totalMaxSpiritualStrength = 99;
            currentMaxPotentialSpiritualStrength = 99;
            spiritualStrength = 99;
        } else {
            totalMaxSpiritualStrength += 10;
            currentMaxPotentialSpiritualStrength =+ 10;
            spiritualStrength += 10;
        }
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        OnPlayerStatsUpdated?.Invoke();
        Debug.Log("totalMaxSpiritualStrength is " + totalMaxSpiritualStrength);
        Debug.Log("currentMaxSpiritualStrength is " + currentMaxPotentialSpiritualStrength);
    }


    public void ModifyPhysicalStrength(int amount)
    {
        Debug.Log("Amount" + amount);
        Debug.Log("physicalStrength Before" + physicalStrength);

        physicalStrength += amount;
        Debug.Log("physicalStrength After" + physicalStrength);
        Debug.Log($"Player PhysicalStrength: {physicalStrength}");
        if (physicalStrength <= 0)
        {
            Die();
        }
        OnPlayerStatsUpdated?.Invoke();
    }


    public void ModifyFloorNumber() {
        gameManager.currentFloor++;
        OnPlayerStatsUpdated?.Invoke();
    }
    
    public void ModifyArrows(int amount) {
        arrows += amount;
        if (arrows < 0) {
            arrows = 0;
        }
        Debug.Log($"Arrows Left = {arrows}");
        OnPlayerStatsUpdated?.Invoke();
    }


    public void ModifyFood(int amount) {
        food += amount;
        OnPlayerStatsUpdated?.Invoke();
    }


    public void ModifyScore(int amount) {
        score += amount;
        OnPlayerStatsUpdated?.Invoke();
    }


    public void ModifyWeaponAttackPower(ItemMapping itemMapping) {
        physicalWeapon = itemMapping.warAttackPower;
        spiritualWeapon = itemMapping.spiritualAttackPower;
        OnPlayerStatsUpdated?.Invoke();
    }


    private void Die()
    {
        Debug.Log("Player Defeated!");

        // Only attempt resurrection if both caps are high enough
        if (currentMaxPotentialPhysicalStrength >= 16 
            && currentMaxPotentialSpiritualStrength >= 9)
        {
            // 75% chance to resurrect
            if (UnityEngine.Random.value <= 0.75f)
            {
                Resurrect();
                return;
            }
        }
        // Otherwise fall back to normal game over
        Debug.Log("Couldn't resurrect");
        gameManager.GameOverSequence();
    }



    private void Resurrect()
    {
        Debug.Log("Resurrection successful!");

        // 1) Clear out the backpack
        inventoryManager.EmptyBackpack();

        // 2) Reset position & rotation
        transform.position = new Vector3(5f, 2.5f, 5f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        // 3) Zero out score
        score = 0;

        // 4) Reset Minimap position
        playerGridMovement.ResetPlayerCursorOnMiniMapOnResurrection();


        // 5) Reset game values 
        gameManager.isFighting = false;
        playerAmbushDetection.ambushTriggered = false; //Allows to be double ambushed once the first ambush ends when caught in between 2 enemiesgit
        gameManager.isExploring = true;
        playerGridMovement.HideActionButton();
        gameManager.SetActiveEnemy(null);  // Clear active enemy
        gameManager.enemyHPText.gameObject.SetActive(false);

        // 6) Notify UI
        gameManager.SetPlayerMessage("Resurrected!");
        OnPlayerStatsUpdated?.Invoke();
    }


    public void CheckIfCanRest() {
        if (((!gameManager.isFighting && (physicalStrength < currentMaxPotentialPhysicalStrength)) || (!gameManager.isFighting && (spiritualStrength < currentMaxPotentialSpiritualStrength))) && food > 0) {
            canRest = true;
        } else {
            canRest = false;
        }
    }


    public void Rest() {
        // Resting.  Resting brings the player current health up towards the current Max Health by using
        // the available food (flour) units


        //Leveling up
        // This works depending on your last battle.  If your last battle you were hit spiritual and you were hit 3 times, when using flour you can increase your max hp to yur current max + 3
        // There are 2 caps which get increased with the books


        //WAR:  Caps - NoBook 49 -> Bluebook 99 -> PinkBook 149 -> Purple 199
        //Spiritual: Caps - NoBook 29 -> Bluebook 49 -> Pink 74 -> Purple 99

        // Blue War Book: Using this book will turn your War HP score blue. After using 
        // War weapons in battle, this book will help you accumulate HP points quicker; 
        // up to a limit of 99 by pressing "rest".

        // Pink War Book: Using this book will turn your War HP score tan. After using 
        // War weapons in battle, this book will help you accumulate HP points quicker; 
        // up to a limit of 149 by pressing "rest".

        // Purple War Book: Using this book will turn your War HP score white. After 
        // using War weapons in battle, this book will help you accumulate HP points 
        // quicker; up to the game limit of 199 in War, by pressing "rest".

        Debug.Log("Player Rested");
        
        // if (!canRest) return;

        // if (lastHitWasWar) {
        //     Debug.Log("Resting War stats");
        //     if (physicalStrength < currentMaxPotentialPhysicalStrength && food > 0) {
        //         while (physicalStrength < currentMaxPotentialPhysicalStrength && food > 0) {
        //             physicalStrength++;
        //             food--;
        //         }
        //     }
        //     canRest = false;
        //     OnPlayerStatsUpdated?.Invoke();
        // } else {
        //     Debug.Log("Resting Spiritual stats");
        //     if (spiritualStrength < currentMaxPotentialSpiritualStrength && food > 0) {
        //         while (spiritualStrength < currentMaxPotentialSpiritualStrength && food > 0) {
        //             spiritualStrength++;
        //             food--;
        //         }
        //     }
        //     canRest = false;
        //     OnPlayerStatsUpdated?.Invoke();
        // }

        ////
        /// UPDATED REST MECHANIC.  Fills up first the one you were hit, but if you have food to spare will also try and fill the other stat. Offers better survivability 
        if (!canRest) return;
        
        Debug.Log("Player Rested");
        
        // first, restore the pool you were last hit in
        if (lastHitWasWar) {
            // how much war HP we still need
            int warNeeded = currentMaxPotentialPhysicalStrength - physicalStrength;
            // eat as many food as will fill that gap (or as much as we have)
            int warToRestore = Mathf.Min(warNeeded, food);
            physicalStrength += warToRestore;
            food             -= warToRestore;
            
            // now, if we still have food left, top up spiritual
            int spiritNeeded  = currentMaxPotentialSpiritualStrength - spiritualStrength;
            int spiritToRestore = Mathf.Min(spiritNeeded, food);
            spiritualStrength += spiritToRestore;
            food              -= spiritToRestore;
            
        } else {
            // last hit was spiritual — do the mirror
            int spiritNeeded   = currentMaxPotentialSpiritualStrength - spiritualStrength;
            int spiritToRestore = Mathf.Min(spiritNeeded, food);
            spiritualStrength  += spiritToRestore;
            food               -= spiritToRestore;
            
            int warNeeded     = currentMaxPotentialPhysicalStrength - physicalStrength;
            int warToRestore   = Mathf.Min(warNeeded, food);
            physicalStrength  += warToRestore;
            food              -= warToRestore;
        }

        // clamp just in case
        physicalStrength  = Mathf.Min(physicalStrength,  currentMaxPotentialPhysicalStrength);
        spiritualStrength = Mathf.Min(spiritualStrength, currentMaxPotentialSpiritualStrength);

        canRest = false;
        OnPlayerStatsUpdated?.Invoke();
    }


    public void CalculateCurrentMaxPotentialHP(string bookName) {
    //Here Im gonna make all the calculations of the potential HPs based on what HP BOOK is active
    /////
    /// WAR BOOKS
        //Book-War-Blue 99
        if (bookName == "Book-War-Blue") {
            currentWarBookCurrentCapHP = 99;
        }
        
        //Book-War-Pink 149
        if (bookName == "Book-War-Pink") {
            currentWarBookCurrentCapHP = 149;
        }

        //Book-War-Purple 199
        if (bookName == "Book-War-Purple") {
            currentWarBookCurrentCapHP = 199;
        }

    /////
    /// SPIRITUAL BOOKS
        //Book-Spiritual-Blue 49
        if (bookName == "Book-Spiritual-Blue") {
                currentSpiritualBookCurrentCapHP = 49;
        }

        //Book-Spiritual-Pink 74
        if (bookName == "Book-Spiritual-Pink") {
                currentSpiritualBookCurrentCapHP = 74;
        }

        //Book-Spiritual-Purple 99
        if (bookName == "Book-Spiritual-Purple") {
                currentSpiritualBookCurrentCapHP = 99;
        }
    }


    public void UpdateUIStats() {
        OnPlayerStatsUpdated?.Invoke();
    }


    public void playerTakeDamageCalculation(ItemMapping itemMapping)
    {
        // Reference the enemy
        Enemy activeEnemy = gameManager.activeEnemy;
        EnemyMapping enemyMapping = activeEnemy.enemyMapping;

        bool isWar = enemyMapping.isWar;
        bool isSpiritual = enemyMapping.isSpiritual;

        // Calculate full attack: base + scaling + color multiplier
        float floorMultiplier = gameManager.currentFloor * enemyMapping.attackPerFloor;
        float baseAttack = enemyMapping.baseAttack + floorMultiplier;
        float rawAttack = baseAttack * enemyMapping.colorMultiplier;

        // Apply bonus randomness
        float bonus = (gameManager.currentFloor > 3) ? UnityEngine.Random.Range(rawAttack * 0.05f, rawAttack * 0.12f) : 0f;
        rawAttack += bonus;

        // Pick correct defense and max defense cap
        float defense = isWar ? physicalArmor : spiritualArmor;
        float maxDefense = isWar ? 119f : 52f;

        if (gameManager.currentFloor <= 3)
            defense += 10; // early game player bonus

        // Normalize defense: 100% mitigation if maxDefense
        float defenseRatio = Mathf.Clamp(defense / maxDefense, 0f, 1f);

        // Final damage
        float finalDamage = rawAttack * (1f - defenseRatio);
        finalDamage = Mathf.Max(finalDamage, 1f); // always deal at least 1 damage
        int finalDamageInt = Mathf.RoundToInt(finalDamage);

        // Apply damage to correct health pool
        if (itemMapping.isWarWeapon)
        {
            physicalStrength -= finalDamageInt;
            lastHitWasWar = true;
            
            // if ((currentMaxPotentialPhysicalStrength + gameManager.WarHPBookMultiplier) <= currentWarBookCurrentCapHP) {
            //     currentMaxPotentialPhysicalStrength = currentMaxPotentialPhysicalStrength + (1 * gameManager.WarHPBookMultiplier);
            // } else {currentMaxPotentialPhysicalStrength = currentWarBookCurrentCapHP; }
            int warIncrement = gameManager.WarHPBookMultiplier;
            if (currentMaxPotentialPhysicalStrength < currentWarBookCurrentCapHP)
            {
                currentMaxPotentialPhysicalStrength 
                = Mathf.Min(
                    currentMaxPotentialPhysicalStrength + warIncrement,
                    currentWarBookCurrentCapHP
                    );
            }  // if you’re already above the cap, do nothing—preserve your hard-earned base.


            Debug.Log($"[PLAYER HIT - WAR] -{finalDamageInt} | HP: {physicalStrength} | DefRatio: {defenseRatio:P0}");
            if (physicalStrength <= 0) Die();
        }
        else if (itemMapping.isSpiritualWeapon)
        {
            spiritualStrength -= finalDamageInt;
            lastHitWasWar = false;

            //USE THE LEVELING UP MECHANICS THAT CAN INVOLVE THE HP BOOKS
            // if ((currentMaxPotentialSpiritualStrength + gameManager.SpiritualHPBookMultiplier) <= currentSpiritualBookCurrentCapHP) {
            //     currentMaxPotentialSpiritualStrength = currentMaxPotentialSpiritualStrength + (1 * gameManager.SpiritualHPBookMultiplier);
            // } else {currentMaxPotentialSpiritualStrength = currentSpiritualBookCurrentCapHP; }
            int spiritIncrement = gameManager.SpiritualHPBookMultiplier;
            if (currentMaxPotentialSpiritualStrength < currentSpiritualBookCurrentCapHP)
            {
                currentMaxPotentialSpiritualStrength 
                = Mathf.Min(
                    currentMaxPotentialSpiritualStrength + spiritIncrement,
                    currentSpiritualBookCurrentCapHP
                    );
            }
            // if you’re already above the cap, do nothing—preserve your hard-earned base.

            Debug.Log($"[PLAYER HIT - SPIRITUAL] -{finalDamageInt} | HP: {spiritualStrength} | DefRatio: {defenseRatio:P0}");
            if (spiritualStrength <= 0) Die();
        }
        else
        {
            Debug.LogWarning("Enemy attack type not set correctly (neither War nor Spiritual).");
        }

        OnPlayerStatsUpdated?.Invoke();
    }


    // public void playerTakeDamageCalculation(ItemMapping itemMapping)
    // {
    //     //Max War Def = 119
    //     //Max Spirit Def = 52
    //     //This logic deals damage based on the ACTIVE ENEMY attack power hitting the player, not the weapon used attack power
    //     Enemy activeEnemy = gameManager.activeEnemy;
    //     EnemyMapping activeEnemyMapping = activeEnemy.enemyMapping;

    //     bool isWar = activeEnemyMapping.isWar;
    //     bool isSpiritual = activeEnemyMapping.isSpiritual;

    //     float baseDamage = activeEnemyMapping.baseAttack;
    //     //float damageSpiritual = itemMapping.spiritualAttackPower;

    //     //float baseDamage = Mathf.Max(damageWar, damageSpiritual);
    //     float bonusDamage = UnityEngine.Random.Range(baseDamage * 0.05f, baseDamage * 0.12f);
        
    //     if (gameManager.currentFloor <=  3) {
    //         bonusDamage = 0;
    //     }
        


    //     float rawAttack = baseDamage + bonusDamage;

    //     // Choose defense based on attack type
    //     float defense = isWar ? physicalArmor : spiritualArmor;

    //     if (gameManager.currentFloor <=  3) {
    //         defense = defense + 5; //Early floors defense bonus
    //     }

    //     float finalDamage = rawAttack * (1 - (defense / 50f));
    //     finalDamage = Mathf.Max(finalDamage, 1); // prevent zero damage

    //     int finalDamageInt = Mathf.RoundToInt(finalDamage);

    //     // Apply damage to correct health pool
    //     if (itemMapping.isWarWeapon)
    //     {
    //         physicalStrength -= finalDamageInt;
    //         Debug.Log($"[PLAYER HIT - WAR] -{finalDamageInt} | Remaining Physical HP: {physicalStrength}  Defense: {defense}");
    //         if (physicalStrength <= 0) Die();
    //     }
    //     else if (itemMapping.isSpiritualWeapon)
    //     {
    //         spiritualStrength -= finalDamageInt;
    //         Debug.Log($"[PLAYER HIT - SPIRITUAL] -{finalDamageInt} | Remaining Spiritual HP: {spiritualStrength}");
    //         if (spiritualStrength <= 0) Die();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Unknown damage type: neither war nor spiritual marked!");
    //     }

    //     // Trigger UI Update
    //     OnPlayerStatsUpdated?.Invoke();
    // }





    // public void playerTakeDamageCalculation(ItemMapping itemMapping)
    // {
    //     //This logic deals damage based on the Weapon attack power used to hit the player, not the enemy attack power
    //     float damageWar = itemMapping.warAttackPower;
    //     float damageSpiritual = itemMapping.spiritualAttackPower;
    //     bool isWar = itemMapping.isWarWeapon;
    //     bool isSpiritual = itemMapping.isSpiritualWeapon;

    //     float baseDamage = Mathf.Max(damageWar, damageSpiritual);
    //     float bonusDamage = UnityEngine.Random.Range(baseDamage * 0.05f, baseDamage * 0.25f);
    //     float rawAttack = baseDamage + bonusDamage;

    //     // Choose defense based on attack type
    //     float defense = isWar ? physicalArmor : spiritualArmor;

    //     float finalDamage = rawAttack * (1 - (defense / 100f));
    //     finalDamage = Mathf.Max(finalDamage, 1); // prevent zero damage

    //     int finalDamageInt = Mathf.RoundToInt(finalDamage);

    //     // Apply damage to correct health pool
    //     if (isWar)
    //     {
    //         physicalStrength -= finalDamageInt;
    //         Debug.Log($"[PLAYER HIT - WAR] -{finalDamageInt} | Remaining Physical HP: {physicalStrength}");
    //         if (physicalStrength <= 0) Die();
    //     }
    //     else if (isSpiritual)
    //     {
    //         spiritualStrength -= finalDamageInt;
    //         Debug.Log($"[PLAYER HIT - SPIRITUAL] -{finalDamageInt} | Remaining Spiritual HP: {spiritualStrength}");
    //         if (spiritualStrength <= 0) Die();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Unknown damage type: neither war nor spiritual marked!");
    //     }

    //     // Trigger UI Update
    //     OnPlayerStatsUpdated?.Invoke();
    // }



}