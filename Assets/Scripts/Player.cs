using UnityEngine;

public class Player : MonoBehaviour
{
    private int totalMaxPhysicalStrength = 199;
    private int totalMaxSpiritualStrength = 99;
    private int totalMaxPhysicalArmor = 199;
    private int totalMaxSpiritualArmor = 99;
    
    public int currentMaxPhysicalStrength = 199;
    public int currentMaxSpiritualStrength = 50;
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
    GameManager gameManager;
    //Events to notify UI changes
    public delegate void OnStatChanged();
    public event OnStatChanged OnPlayerStatsUpdated;


    void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        physicalStrength = 900;
        physicalArmor = 0;
        physicalWeapon = 10;
        spiritualStrength = 50;
        spiritualArmor = 0;
        spiritualWeapon = 1;
        score = 0;
        arrows = 10;
        food = 100;
        gameManager.currentFloor = 1;

        //Trigger UI update at start
        OnPlayerStatsUpdated?.Invoke();
    }


    public void RestoreMaxPhysicalStrengthWithSmallBluePotion() {
        physicalStrength = currentMaxPhysicalStrength;
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        OnPlayerStatsUpdated?.Invoke();
    }


    public void IncreasePhysicalScoreBy10WithLargeBluePotion() {
        if (physicalStrength + 10 >= 199) {
            totalMaxPhysicalStrength = 199;
            currentMaxPhysicalStrength = 199;
            physicalStrength = 199;
        } else {
            totalMaxSpiritualStrength += 10;
            currentMaxPhysicalStrength += 10;
            physicalStrength += 10;
        }
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        Debug.Log("totalMaxPhysicalStrength is " + totalMaxSpiritualStrength);
        Debug.Log("CurrentMaxPhysicalStrength is " + currentMaxPhysicalStrength);

        OnPlayerStatsUpdated?.Invoke();
    }


    public void IncreaseSpiritualScoreBy10WithLargePinkPotion() {
        if (spiritualStrength + 10 >= 99) {
            totalMaxSpiritualStrength = 99;
            currentMaxSpiritualStrength = 99;
            spiritualStrength = 99;
        } else {
            totalMaxSpiritualStrength += 10;
            currentMaxSpiritualStrength =+ 10;
            spiritualStrength += 10;
        }
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        OnPlayerStatsUpdated?.Invoke();
        Debug.Log("totalMaxSpiritualStrength is " + totalMaxSpiritualStrength);
        Debug.Log("currentMaxSpiritualStrength is " + currentMaxSpiritualStrength);
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
        // Add logic for player death, like restarting the level or showing a game-over screen
        //gameObject.SetActive(false);
        gameManager.GameOverSequence();
    }


    public void CheckIfCanRest() {
        if (((!gameManager.isFighting && (physicalStrength < currentMaxPhysicalStrength)) || (!gameManager.isFighting && (spiritualStrength < currentMaxSpiritualStrength))) && food > 0) {
            canRest = true;
        } else {
            canRest = false;
        }
    }


    public void Rest() {
        // Resting.  Resting brings the player current health up towards the current Max Health by using
        // the available food (flour) units
        Debug.Log("Player Rested");
        if (physicalStrength < currentMaxPhysicalStrength && food > 0) {
            while (physicalStrength < currentMaxPhysicalStrength && food > 0) {
                physicalStrength++;
                food--;
            }
        }
        canRest = false;
        OnPlayerStatsUpdated?.Invoke();
    }


    public void UpdateUIStats() {
        OnPlayerStatsUpdated?.Invoke();
    }


    public void playerTakeDamageCalculation(ItemMapping itemMapping)
    {
        float damageWar = itemMapping.warAttackPower;
        float damageSpiritual = itemMapping.spiritualAttackPower;
        bool isWar = itemMapping.isWarWeapon;
        bool isSpiritual = itemMapping.isSpiritualWeapon;

        float baseDamage = Mathf.Max(damageWar, damageSpiritual);
        float bonusDamage = UnityEngine.Random.Range(baseDamage * 0.05f, baseDamage * 0.25f);
        float rawAttack = baseDamage + bonusDamage;

        // Choose defense based on attack type
        float defense = isWar ? physicalArmor : spiritualArmor;

        float finalDamage = rawAttack * (1 - (defense / 100f));
        finalDamage = Mathf.Max(finalDamage, 1); // prevent zero damage

        int finalDamageInt = Mathf.RoundToInt(finalDamage);

        // Apply damage to correct health pool
        if (isWar)
        {
            physicalStrength -= finalDamageInt;
            Debug.Log($"[PLAYER HIT - WAR] -{finalDamageInt} | Remaining Physical HP: {physicalStrength}");
            if (physicalStrength <= 0) Die();
        }
        else if (isSpiritual)
        {
            spiritualStrength -= finalDamageInt;
            Debug.Log($"[PLAYER HIT - SPIRITUAL] -{finalDamageInt} | Remaining Spiritual HP: {spiritualStrength}");
            if (spiritualStrength <= 0) Die();
        }
        else
        {
            Debug.LogWarning("Unknown damage type: neither war nor spiritual marked!");
        }

        // Trigger UI Update
        OnPlayerStatsUpdated?.Invoke();
    }


}