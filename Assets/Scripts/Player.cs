using System;
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
    
    private int currentMaxPotentialPhysicalStrength; //12; //Initial no book cap
    private int currentMaxPotentialSpiritualStrength; //6; //Initial no booko cap
    
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
    
    public Vector3 initialPlayerPosition = new Vector3(5,2.5f,5);
    public Vector3 initialPlayerRotation = new Vector3(0,0,0);

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
    // INITIAL HEALTH (War/spiritual), FOOD/ARROWS, VULNERABILITY

    // "3", EASY, 2, 18/9, 9/9, 1/4
    // "2", NORMAL, 4, 16/8, 8/8, 1/2
    // "1", HARD, 8, 14/7, 7/7, 3/4
    // disc/other, VERY-HARD, 12, 12/6, 6/6, FULL

        //Initial STATS based on difficulty
        switch (gameManager.CurrentDifficulty)
        {
            case DifficultyLevel.VeryHard:
                physicalStrength = 12;
                spiritualStrength = 6;
                food = 6;
                arrows = 6;
                currentMaxPotentialPhysicalStrength = 12;
                currentMaxPotentialSpiritualStrength = 6;
                break;
            case DifficultyLevel.Hard:
                physicalStrength = 14;
                spiritualStrength = 7;
                food = 7;
                arrows = 7;
                currentMaxPotentialPhysicalStrength = 14;
                currentMaxPotentialSpiritualStrength = 7;
                break;
            case DifficultyLevel.Normal:
                physicalStrength = 16;
                spiritualStrength = 8;
                food = 8;
                arrows = 8;
                currentMaxPotentialPhysicalStrength = 16;
                currentMaxPotentialSpiritualStrength = 8;
                break;
            case DifficultyLevel.Easy:
                physicalStrength = 18;
                spiritualStrength = 9;
                food = 9;
                arrows = 9;
                currentMaxPotentialPhysicalStrength = 18;
                currentMaxPotentialSpiritualStrength = 9;
                break;
        }

        //physicalStrength = 12; // 12
        physicalArmor = 0;
        physicalWeapon = 0;
        //spiritualStrength = 6; // 6
        spiritualArmor = 0;
        spiritualWeapon = 0;
        score = 0;
        //arrows = 10;
        //food = 10;
        gameManager.currentFloor = 1;

        //Trigger UI update at start
        OnPlayerStatsUpdated?.Invoke();
    }


    public void RestoreMaxPhysicalStrengthWithSmallBluePotion() {
        //Restores both Physical and Spiritual
        physicalStrength = currentMaxPotentialPhysicalStrength;
        spiritualStrength = currentMaxPotentialSpiritualStrength;
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        gameManager.PlayWhooshSoundEffect();
        OnPlayerStatsUpdated?.Invoke();
    }


    public void IncreasePhysicalScoreBy10WithLargeBluePotion() {
        if (physicalStrength + 10 >= 199) {
            totalMaxPhysicalStrength = 199;
            currentMaxPotentialPhysicalStrength = 199;
            physicalStrength = 199;
        } else {
            //totalMaxSpiritualStrength += 10;
            currentMaxPotentialPhysicalStrength += 10;
            physicalStrength = currentMaxPotentialPhysicalStrength; //Refill all the way up
        }
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        Debug.Log("totalMaxPhysicalStrength is " + totalMaxPhysicalStrength);
        Debug.Log("CurrentMaxPhysicalStrength is " + currentMaxPotentialPhysicalStrength);

        OnPlayerStatsUpdated?.Invoke();
    }


    public void IncreaseSpiritualScoreBy10WithLargePinkPotion() {
        if (spiritualStrength + 10 >= 99) {
            totalMaxSpiritualStrength = 99;
            currentMaxPotentialSpiritualStrength = 99;
            spiritualStrength = 99;
        } else {
            //totalMaxSpiritualStrength += 10;
            currentMaxPotentialSpiritualStrength += 10;
            spiritualStrength = currentMaxPotentialSpiritualStrength;
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
        if (gameManager.currentFloor == 255) {  //Testing GamePLUS should be 255
            gameManager.currentFloor = 1;
        } else {
            gameManager.currentFloor++;
            gameManager.PlayLadderSoundEffect();
        }        
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
        playerGridMovement.CancelEscape(); //Stops the Escape coroutine to prevent Die and Escape to happen at the same time.  If the player dies on the last escaping hit the player still dies and may or may not resurrect.

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
        gameManager.PlayResurrectSoundEffect();
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

        // 6) Resurrect at 50% of each of your max HP
        physicalStrength = currentMaxPotentialPhysicalStrength / 2;
        spiritualStrength = currentMaxPotentialSpiritualStrength / 2;

        // 6) Notify UI
        gameManager.SetPlayerMessage("Resurrected!");
        OnPlayerStatsUpdated?.Invoke();

        //7 Update the cursor on Minimap
        playerGridMovement.UpdateMinimapCursor(0.0f); //0.0 is an arbitrary value to reset the player to the start of the maze on miinimap
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

        //Debug.Log("Player Rested");
        
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
        
        gameManager.PlayClickSoundEffect();
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
        //On enemy mappings:
        //Floor-scaling & color multipliers unchanged: tune mapping.attackPerFloor (e.g. 0.1 for trash, 0.3 elites)

        // 1) Get the active enemy and its mapping
        Enemy activeEnemy    = gameManager.activeEnemy;
        EnemyMapping mapping = activeEnemy.enemyMapping;
        bool isWarAttack     = mapping.isWar;

        // 2) Compute raw attack = (base + floor scaling) × color multiplier
        float floorBonus = gameManager.currentFloor * mapping.attackPerFloor;
        float baseAttack = mapping.baseAttack + floorBonus;
        float rawAttack  = baseAttack * mapping.colorMultiplier;

        // 3) (Optional) tiny random variation—disabled for predictability
        float rnd = UnityEngine.Random.Range(rawAttack * 0.00f, rawAttack * 0.03f);
        rawAttack += rnd;

        // 4) Pick the correct defense stat
        float defense = isWarAttack 
                        ? physicalArmor 
                        : spiritualArmor;

        // 5) Apply your lowered max-def caps
        float maxDefCap = isWarAttack 
                        ? 119f 
                        : 52f;
                        //? 80f 
                        //: 40f;
                //    ? 119f      // your absolute max war-defense 
                //    :  52f;     // your absolute max spirit-defense
                

        // 6) Compute mitigation ratio, capped at 100%
        float mitigation = Mathf.Clamp(defense / maxDefCap, 0f, 1f);

        // 7) Final damage after mitigation (can be zero!)
        float dmg = rawAttack * (1f - mitigation);

        // 8) Convert to integer.  Use Floor so 0.9→0; you still “hit” but take no HP loss.
        int finalDamage = Mathf.FloorToInt(dmg);

        if (finalDamage == 0) gameManager.SetPlayerMessage("Blocked!");

        // 8.5 Build patterns based on difficulty
        switch (gameManager.CurrentDifficulty)
        {
            case DifficultyLevel.VeryHard:
                finalDamage = Mathf.FloorToInt(finalDamage * 0.90f); //No change
                break;
            case DifficultyLevel.Hard:
                finalDamage = Mathf.FloorToInt(finalDamage * 0.80f);
                break;
            case DifficultyLevel.Normal:
                finalDamage = Mathf.FloorToInt(finalDamage * 0.70f);
                break;
            case DifficultyLevel.Easy:
                finalDamage = Mathf.FloorToInt(finalDamage * 0.60f);
                break;
        }


        //REDUCES MINOTAUR DAMAGE ON EASIER DIFFICULTIES
        if (gameManager.activeEnemy.enemyMapping.name.Contains("Minotaur")) {
            if (gameManager.currentFloor == 2) { //Very Easy
                finalDamage = finalDamage / 4;
            } else if (gameManager.currentFloor == 4) { //Normal
                finalDamage = finalDamage / 3;
            } else if (gameManager.currentFloor == 8) { //Normal
                finalDamage = finalDamage / 2;
            }
        }

        // 9) Apply to the correct health pool and still do your HP‐book increment
        if (itemMapping.isWarWeapon)
        {
            physicalStrength -= finalDamage;
            lastHitWasWar     = true;

            // HP‐book leveling
            int inc = gameManager.WarHPBookMultiplier;
            if (currentMaxPotentialPhysicalStrength < currentWarBookCurrentCapHP)
            {
                currentMaxPotentialPhysicalStrength = Mathf.Min(
                    currentMaxPotentialPhysicalStrength + inc,
                    currentWarBookCurrentCapHP
                );
            }

            Debug.Log($"[PLAYER HIT-WAR] -{finalDamage} HP | Mitigation: {mitigation:P0}");
            if (physicalStrength <= 0) Die();
        }
        else if (itemMapping.isSpiritualWeapon)
        {
            spiritualStrength -= finalDamage;
            lastHitWasWar       = false;

            int inc = gameManager.SpiritualHPBookMultiplier;
            if (currentMaxPotentialSpiritualStrength < currentSpiritualBookCurrentCapHP)
            {
                currentMaxPotentialSpiritualStrength = Mathf.Min(
                    currentMaxPotentialSpiritualStrength + inc,
                    currentSpiritualBookCurrentCapHP
                );
            }

            Debug.Log($"[PLAYER HIT-SPIRIT] -{finalDamage} HP | Mitigation: {mitigation:P0}");
            if (spiritualStrength <= 0) Die();
        }
        else
        {
            Debug.LogWarning("Enemy attack type not set correctly.");
        }

        // 10) Update UI
        OnPlayerStatsUpdated?.Invoke();
    }



    // public void playerTakeDamageCalculation(ItemMapping itemMapping)
    // {
    //     // Reference the enemy
    //     Enemy activeEnemy = gameManager.activeEnemy;
    //     EnemyMapping enemyMapping = activeEnemy.enemyMapping;

    //     bool isWar = enemyMapping.isWar;
    //     bool isSpiritual = enemyMapping.isSpiritual;

    //     // Calculate full attack: base + scaling + color multiplier
    //     float floorMultiplier = gameManager.currentFloor * enemyMapping.attackPerFloor;
    //     float baseAttack = enemyMapping.baseAttack + floorMultiplier;
    //     float rawAttack = baseAttack * enemyMapping.colorMultiplier;

    //     // Apply bonus randomness
    //     float bonus = (gameManager.currentFloor > 3) ? UnityEngine.Random.Range(rawAttack * 0.05f, rawAttack * 0.12f) : 0f;
    //     rawAttack += bonus;

    //     // Pick correct defense and max defense cap
    //     float defense = isWar ? physicalArmor : spiritualArmor;
    //     float maxDefense = isWar ? 119f : 52f;

    //     if (gameManager.currentFloor <= 3)
    //         defense += 10; // early game player bonus

    //     // Normalize defense: 100% mitigation if maxDefense
    //     float defenseRatio = Mathf.Clamp(defense / maxDefense, 0f, 1f);

    //     // Final damage
    //     float finalDamage = rawAttack * (1f - defenseRatio);
    //     finalDamage = Mathf.Max(finalDamage, 1f); // always deal at least 1 damage
    //     int finalDamageInt = Mathf.RoundToInt(finalDamage);

    //     // Apply damage to correct health pool
    //     if (itemMapping.isWarWeapon)
    //     {
    //         physicalStrength -= finalDamageInt;
    //         lastHitWasWar = true;
            
    //         int warIncrement = gameManager.WarHPBookMultiplier;
    //         if (currentMaxPotentialPhysicalStrength < currentWarBookCurrentCapHP)
    //         {
    //             currentMaxPotentialPhysicalStrength 
    //             = Mathf.Min(
    //                 currentMaxPotentialPhysicalStrength + warIncrement,
    //                 currentWarBookCurrentCapHP
    //                 );
    //         }  // if you’re already above the cap, do nothing—preserve your hard-earned base.


    //         Debug.Log($"[PLAYER HIT - WAR] -{finalDamageInt} | HP: {physicalStrength} | DefRatio: {defenseRatio:P0}");
    //         if (physicalStrength <= 0) Die();
    //     }
    //     else if (itemMapping.isSpiritualWeapon)
    //     {
    //         spiritualStrength -= finalDamageInt;
    //         lastHitWasWar = false;

    //         //USE THE LEVELING UP MECHANICS THAT CAN INVOLVE THE HP BOOKS
    //         int spiritIncrement = gameManager.SpiritualHPBookMultiplier;
    //         if (currentMaxPotentialSpiritualStrength < currentSpiritualBookCurrentCapHP)
    //         {
    //             currentMaxPotentialSpiritualStrength 
    //             = Mathf.Min(
    //                 currentMaxPotentialSpiritualStrength + spiritIncrement,
    //                 currentSpiritualBookCurrentCapHP
    //                 );
    //         }
    //         // if you’re already above the cap, do nothing—preserve your hard-earned base.
    //         Debug.Log($"[PLAYER HIT - SPIRITUAL] -{finalDamageInt} | HP: {spiritualStrength} | DefRatio: {defenseRatio:P0}");
    //         if (spiritualStrength <= 0) Die();
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Enemy attack type not set correctly (neither War nor Spiritual).");
    //     }

    //     OnPlayerStatsUpdated?.Invoke();
    // }






    public void CorridorDoorCrossingHP(string crossedDoor)
    {
        Debug.Log($"Crossed door: {crossedDoor} Will do logic later");

        return; //NOT USING THIS LOGIC FOR NOW
        
        Debug.Log($"Crossed door: {crossedDoor}");

        if (crossedDoor.Contains("Blue"))
        {
            // transfer all War HP into Spiritual HP
            int transfer = physicalStrength;
            physicalStrength = 0;
            spiritualStrength = Mathf.Min(
                currentMaxPotentialSpiritualStrength,
                spiritualStrength + transfer
            );
            Debug.Log($"Blue door: war→spiritual transfer {transfer}");
        }
        else if (crossedDoor.Contains("Green"))
        {
            // transfer all Spiritual HP into War HP
            int transfer = spiritualStrength;
            spiritualStrength = 0;
            physicalStrength = Mathf.Min(
                currentMaxPotentialPhysicalStrength,
                physicalStrength + transfer
            );
            Debug.Log($"Green door: spiritual→war transfer {transfer}");
        }
        else
        {
            // you could handle Tan (or other) doors here if you like, or leave them alone
            Debug.Log("Other door — no HP swap");
        }

        // update the UI
        OnPlayerStatsUpdated?.Invoke();
    }


    public void ConsumeLargePurplePotion()
    {
    //Large Purple: Swap war/spiritual health ratios (uses one turn in battle):
    //  - values are found from war/199 and spiritual/99
    //  - ie: if your health is 100/40, this is 100/199 war and 40/99 spiritual... about 50% and 40% respectively.  
    //   This potion swaps these percentages, changing your health to 40% and 50%... about 80/50.

        float currentPhysicalPercentageFromMax;
        float currentSpiritualPercentageFromMax;

        int atCalculationCurrentMaxPotentialPhysicalStrength = currentMaxPotentialPhysicalStrength;
        int atCalculationCurrentMaxPotentialSpiritualStrength = currentMaxPotentialSpiritualStrength;
        // What percentage is 100 of 199
        // 199  100%
        // 100   X    (100 X 100 / 199) = 50.25
        // Then calculate 50.25% of 99 (Spiritual) and assign to SpiritualCurrentMax
        // 50.25% of 99 = 49.74 Ronund to 50
        Debug.Log("currentMaxPotentialSpiritualStrength" + currentMaxPotentialSpiritualStrength);
        currentPhysicalPercentageFromMax = (atCalculationCurrentMaxPotentialPhysicalStrength * 100) / 199; //199
        currentMaxPotentialSpiritualStrength = (int)Math.Round(99 * (currentPhysicalPercentageFromMax/100));
        Debug.Log("currentMaxPotentialSpiritualStrength" + currentMaxPotentialSpiritualStrength);
        Debug.Log("currentPhysicalPercentageFromMax" + currentPhysicalPercentageFromMax);
        Debug.Log("currentMaxPotentialPhysicalStrength" + currentMaxPotentialPhysicalStrength);
        //Max HPs to current new max:
        spiritualStrength = currentMaxPotentialSpiritualStrength;

        // What percentage is 40 of 99
        //  99  100%
        //  40  x    (40 x 100 / 99) = 40.40%
        //  Then calculate 40.0% of 199 (Physical) and assign to PhysicalCurrentMax
        // 40.4% of 199 = 80.40 rounds to 80   
        Debug.Log("currentMaxPotentialPhysicalStrength" + currentMaxPotentialPhysicalStrength);
        currentSpiritualPercentageFromMax = (atCalculationCurrentMaxPotentialSpiritualStrength * 100) / 99; //99
        currentMaxPotentialPhysicalStrength = (int)Math.Round(199 * (currentSpiritualPercentageFromMax/100));
        Debug.Log("currentMaxPotentialPhysicalStrength" + currentMaxPotentialPhysicalStrength);
        Debug.Log("currentSpiritualPercentageFromMax" + currentSpiritualPercentageFromMax);
        Debug.Log("currentMaxPotentialSpiritualStrength" + currentMaxPotentialSpiritualStrength);
        //Max HPs to current new max:
        physicalStrength = currentMaxPotentialPhysicalStrength;
        
        // OLD WRONG WAY
        // If War > Spirit: halve War, add half of *that* loss to Spirit
        // if (physicalStrength > spiritualStrength)
        // {
        //     int warLost    = physicalStrength / 2;      // integer division rounds down
        //     physicalStrength -= warLost;
        //     int spiritGain = warLost / 2;              // half of what was lost
        //     spiritualStrength = Mathf.Min(
        //         currentMaxPotentialSpiritualStrength,
        //         spiritualStrength + spiritGain
        //     );
        //     Debug.Log($"Purple potion: War→Spirit: War lost {warLost}, Spirit gained {spiritGain}");
        // }
        // // Else if Spirit > War: halve Spirit, add double that loss to War
        // else if (spiritualStrength > physicalStrength)
        // {
        //     int spiritLost = spiritualStrength / 2;
        //     spiritualStrength -= spiritLost;
        //     int warGain    = spiritLost * 2;           // double what was lost
        //     physicalStrength = Mathf.Min(
        //         currentMaxPotentialPhysicalStrength,
        //         physicalStrength + warGain
        //     );
        //     Debug.Log($"Purple potion: Spirit→War: Spirit lost {spiritLost}, War gained {warGain}");
        // }
        // else
        // {
        //     Debug.Log("Purple potion: pools equal, no change");
        // }

        // Drinking takes your turn
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn  = true;

        OnPlayerStatsUpdated?.Invoke();
    }


    public void PlacePlayerAtStartOnGamePlus() {
        transform.position =  initialPlayerPosition;
        transform.eulerAngles = initialPlayerRotation;
    }

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



